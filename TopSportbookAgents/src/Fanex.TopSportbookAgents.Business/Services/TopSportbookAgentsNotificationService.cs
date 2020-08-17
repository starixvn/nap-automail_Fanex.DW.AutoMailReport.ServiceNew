using Fanex.LetstalkService.Client;
using Fanex.MailDelivery.Core.Models;
using Fanex.TopSportbookAgents.Business.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Fanex.TopSportbookAgents.Business.Services
{
    public interface TopSportbookAgentsNotificationService
    {
        JobMessage Execute();
    }

    public class TopSportbookAgentsNotificationServiceImp : TopSportbookAgentsNotificationService
    {
        private readonly Log log;
        private readonly DataService dataService;
        private readonly LetstalkClientService letstalkService;
        private readonly ExcelService excelService;
        private readonly MessageService messageService;
        private readonly ImageService imageService;
        private readonly FileService fileService;
        private readonly MailService mailService;
        private readonly DateRangeService dateRangeService;
        private List<string> filePaths = new List<string>();
        private Mail mail = new Mail() { Subject = string.Empty, Body = string.Empty };

        public TopSportbookAgentsNotificationServiceImp(
            Log log,
            DataService dataService,
            LetstalkClientService letstalkService,
            ExcelService excelService,
            MessageService messageService,
            ImageService imageService,
            FileService fileService,
            MailService mailService,
            DateRangeService dateRangeService)
        {
            this.log = log;
            this.dataService = dataService;
            this.letstalkService = letstalkService;
            this.excelService = excelService;
            this.messageService = messageService;
            this.imageService = imageService;
            this.fileService = fileService;
            this.mailService = mailService;
            this.dateRangeService = dateRangeService;
        }

        public JobMessage Execute()
        {
            var result = new ExecuteResultModel();
            JobMessage jobMessage = new JobMessage();
            try
            {
                result = SendNotification();
            }
            catch (Exception e)
            {
                result.Success = false;
                result.Message = e.Message;
                log.Log(e);
            }
            finally
            {
                jobMessage.Mail = result.Mail;
                jobMessage.JobStatus = result.Success ? JobStatus.Success : JobStatus.DataIsNotReady;
                jobMessage.TaskLog = new TaskLog
                {
                    LogTime = DateTime.Now,
                    Message = result.Message
                };
            }

            return jobMessage;
        }

        private ExecuteResultModel SendNotification()
        {
            if (!dataService.IsDataAvailable())
            {
                return new ExecuteResultModel { Success = false, Message = "DataIsNotReady" };
            }

            var letstalkFails = new List<string>();
            var rawData = dataService.QueryTopSportbookAgents(AppConfigService.Config.WinLoss);
            var errFiles = rawData.Count > 0
                ? SendLetstalkImageData(rawData)
                : SendLetstalkImageNoData();

            errFiles.ForEach(file =>
            {
                if (file.Length > 1)
                {
                    letstalkFails.Add(file);
                }
            });

            if (!AppConfigService.Config.IsSentDirectly)
            {
                fileService.CopyFiles(filePaths);
            }

            return new ExecuteResultModel
            {
                Success = letstalkFails.Count == 0,
                Message = string.Join(Environment.NewLine, letstalkFails),
                Mail = mail
            };
        }

        private List<string> SendLetstalkImageData(List<PunterModel> data)
        {
            var rowPerBatch = 50;
            var totalBatch = data.Count / rowPerBatch + ((data.Count % rowPerBatch) > 0 ? 1 : 0);
            var imageSentFail = new List<string>();
            var countRetry = 0;
            var letstalkFails = new List<string>();
            var setting = AppConfigService.Config.TopSportbookAgentSettings;

            for (var batchIndex = 0; batchIndex < totalBatch; batchIndex++)
            {
                var rowsOfBatch = data.Skip(batchIndex * rowPerBatch).Take(rowPerBatch).ToList();
                var excelPath = excelService.ExportExcel(rowsOfBatch, batchIndex, totalBatch, false, setting);
                var imagePath = ExportImage(setting, excelPath, batchIndex);
                var status = SendImage(imagePath, setting.LetstalkGroup);
                countRetry = 0;
                while (!status && countRetry < 10)
                {
                    status = SendImage(imagePath, setting.LetstalkGroup);
                    countRetry++;
                }

                if (!status)
                {
                    imageSentFail.Add(imagePath);
                }
            }

            var excelAllPath = excelService.ExportExcel(data, 0, 0, true, setting);
            SendImage(excelAllPath, setting.LetstalkGroup);
            SendMail(setting, excelAllPath);

            if (imageSentFail.Count > 0)
            {
                letstalkFails.Add($"{string.Join(Environment.NewLine, imageSentFail)} - Letstalk sent images Unsuccessfully.");
            }

            fileService.MoveFiles(AppConfigService.Config.InternalPath, AppConfigService.Config.Path);

            return imageSentFail;
        }

        private void SendMail(TopSportbookAgentSettings setting, string excelAllPath)
        {
            mail = mailService.CreateMail(excelAllPath, setting.Exceltitle, setting.Subject, dateRangeService.RptDate);
        }

        private List<string> SendLetstalkImageNoData()
        {
            var setting = AppConfigService.Config.TopSportbookAgentSettings;
            var excelPath = excelService.ExportExcelWithNoData(setting);
            var imagePath = ExportImage(setting, excelPath, 0);
            var status = SendImage(imagePath, setting.LetstalkGroup);
            SendMail(setting, excelPath);
            return new List<string> { status ? string.Empty : $"{imagePath} - Letstalk sent images Unsuccessfully." };
        }

        private string ExportImage(TopSportbookAgentSettings setting, string excelPath, int batchIndex)
        {
            var image = AppConfigService.Config.ImageSettings["TopSportbookAgents"];
            image.ImageName = setting.Exceltitle;
            var imagePath = imageService.ExportImage(excelPath, batchIndex, image.SheetName, image);

            return imagePath;
        }

        private bool SendImage(string imagePath, string group)
        {
            var letstalkGroup = group;
            var letstalkModel = messageService.CreateLetstalkModel(imagePath, letstalkGroup);
            var status = false;

            if (AppConfigService.Config.IsSentDirectly)
            {
                status = letstalkService.SendMessage(letstalkModel);
            }
            else
            {
                status = true;
            }

            filePaths.Add(imagePath);

            return status;
        }
    }
}