using Fanex.HouseMarginMonitoring.Business.Models;
using Fanex.LetstalkService.Client;
using Fanex.MailDelivery.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Fanex.HouseMarginMonitoring.Business.Services
{
    public interface NotificationService
    {
        JobMessage Execute();
    }

    public class NotificationServiceImp : NotificationService
    {
        private readonly Log log;
        private readonly DataService dataService;
        private readonly LetstalkClientService letstalkService;
        private readonly ExcelService excelService;
        private readonly MessageService messageService;
        private readonly ImageService imageService;
        private readonly FileService fileService;
        private readonly MailService mailService;

        private List<string> filePaths = new List<string>();
        private Mail mail = new Mail { Subject = string.Empty, Body = string.Empty };

        public NotificationServiceImp(
            Log log,
            DataService dataService,
            LetstalkClientService letstalkService,
            ExcelService excelService,
            MessageService messageService,
            ImageService imageService,
            FileService fileService,
            MailService mailService)
        {
            this.log = log;
            this.dataService = dataService;
            this.letstalkService = letstalkService;
            this.excelService = excelService;
            this.messageService = messageService;
            this.imageService = imageService;
            this.fileService = fileService;
            this.mailService = mailService;
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
                jobMessage.JobStatus = result.Success ? JobStatus.SuccessWithoutSendmail : JobStatus.DataIsNotReady;
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
            var letstalkFails = new List<string>();
            var rawData = dataService.QueryData();
            var errFiles = rawData.Count > 1
                ? SendLetstalkImageData(rawData)
                : SendLetstalkImageNoData();

            errFiles.ForEach(file =>
            {
                if (file.Length > 1)
                {
                    letstalkFails.Add(file);
                }
            });

            return new ExecuteResultModel
            {
                Success = letstalkFails.Count == 0,
                Message = string.Join(Environment.NewLine, letstalkFails),
                Mail = mail
            };
        }

        private List<string> SendLetstalkImageData(List<BettypeModel> data)
        {
            var rowPerBatch = data.Count;
            var totalBatch = data.Count / rowPerBatch + ((data.Count % rowPerBatch) > 0 ? 1 : 0);
            var imageSentFail = new List<string>();
            var countRetry = 0;
            var letstalkFails = new List<string>();
            var setting = AppConfigService.Config.HouseMarginMonitoringSettings;

            for (var batchIndex = 0; batchIndex < totalBatch; batchIndex++)
            {
                var rowsOfBatch = data.Skip(batchIndex * rowPerBatch).Take(rowPerBatch).ToList();
                var excelPath = excelService.ExportExcel(rowsOfBatch, batchIndex, totalBatch, false, setting);
                var rowCount = rowsOfBatch.Count;
                var imagePath = ExportImage(setting, excelPath, batchIndex, rowCount);
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
            SendMail(setting, excelAllPath);

            if (imageSentFail.Count > 0)
            {
                letstalkFails.Add($"{string.Join(Environment.NewLine, imageSentFail)} - Letstalk sent images Unsuccessfully.");
            }

            fileService.MoveFiles(AppConfigService.Config.InternalPath, AppConfigService.Config.Path);

            return imageSentFail;
        }

        private void SendMail(HouseMarginSettings setting, string excelAllPath)
        {
            mailService.SendMail(excelAllPath, setting.Exceltitle, setting.Subject, DateTime.Now);
        }

        private List<string> SendLetstalkImageNoData()
        {
            var setting = AppConfigService.Config.HouseMarginMonitoringSettings;
            var excelPath = excelService.ExportExcelWithNoData(setting);
            var imagePath = ExportImage(setting, excelPath, 0, 0);
            var status = true;
            SendMail(setting, excelPath);
            return new List<string> { status ? string.Empty : $"{imagePath} - Letstalk sent images Unsuccessfully." };
        }

        private string ExportImage(HouseMarginSettings setting, string excelPath, int batchIndex, int rowCount)
        {
            var lastRow = rowCount + (rowCount > 0 ? 3 : 2);
            var image = AppConfigService.Config.ImageSettings["HouseMarginMonitoringSettings"];
            image.ImageName = setting.Exceltitle;
            image.LastRow = lastRow;
            image.CropHeight = lastRow * image.HeightPerRow;
            var imagePath = imageService.ExportImage(excelPath, batchIndex, image.SheetName, image);

            return imagePath;
        }

        private bool SendImage(string imagePath, string group)
        {
            var letstalkGroup = group;
            var letstalkModel = messageService.CreateLetstalkModel(imagePath, letstalkGroup);
            var status = letstalkService.SendMessage(letstalkModel);

            filePaths.Add(imagePath);

            return status;
        }
    }
}