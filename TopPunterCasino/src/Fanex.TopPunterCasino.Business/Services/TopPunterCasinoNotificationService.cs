using Fanex.LetstalkService.Client;
using Fanex.MailDelivery.Core.Models;
using Fanex.TopPunterCasino.Business.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Fanex.TopPunterCasino.Business.Services
{
    public interface TopPunterCasinoNotificationService
    {
        JobMessage Execute();
    }

    public class TopPunterCasinoNotificationServiceImp : TopPunterCasinoNotificationService
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
        private List<string> filesPath = new List<string>();

        public TopPunterCasinoNotificationServiceImp(
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
            if (!dataService.IsDataAvailable())
            {
                return new ExecuteResultModel { Success = false, Message = "DataIsNotReady" };
            }

            var letstalkFails = new List<string>();
            var rawData = dataService.QueryCasinoTopPunterData(AppConfigService.Config.TotalTurnover, AppConfigService.Config.NonSportProductIds);
            foreach (var casinoType in AppConfigService.Config.CasinoTypes)
            {
                var data = rawData.Where(s => s.CasinoType == casinoType.Key).ToList();
                var errFiles = data.Count > 0
                    ? SendLetstalkImageData(data, casinoType.Value)
                    : SendLetstalkImageNoData(casinoType.Value);

                errFiles.ForEach(file =>
                {
                    if (file.Length > 1)
                    {
                        letstalkFails.Add(file);
                    }
                });
            }

            if (!AppConfigService.Config.IsSentDirectly)
            {
                var letstalkGroup = AppConfigService.Config.CasinoTypes[CasinoType.RNGCasino].LetstalkGroup;
                fileService.CopyFiles(filesPath, letstalkGroup);
            }

            return new ExecuteResultModel
            {
                Success = letstalkFails.Count == 0,
                Message = string.Join(Environment.NewLine, letstalkFails),
                Mail = new Mail()
            };
        }

        private List<string> SendLetstalkImageData(List<PunterModel> data, TopPunterCasinoSetting setting)
        {
            var rowPerBatch = 50;
            var totalBatch = data.Count / rowPerBatch + ((data.Count % rowPerBatch) > 0 ? 1 : 0);
            var imageSentFail = new List<string>();
            var countRetry = 0;
            var letstalkFails = new List<string>();

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

        private void SendMail(TopPunterCasinoSetting setting, string excelAllPath)
        {
            var mail = mailService.CreateMail(excelAllPath, setting.Exceltitle, setting.Subject, dateRangeService.RptDate);
            mail.MailingListId = setting.MailFunctionId;
            mailService.SendMail(mail);
        }

        private List<string> SendLetstalkImageNoData(TopPunterCasinoSetting setting)
        {
            var excelPath = excelService.ExportExcelWithNoData(setting);
            var imagePath = ExportImage(setting, excelPath, 0);
            var status = SendImage(imagePath, setting.LetstalkGroup);
            SendMail(setting, excelPath);
            return new List<string> { status ? string.Empty : $"{imagePath} - Letstalk sent images Unsuccessfully." };
        }

        private string ExportImage(TopPunterCasinoSetting setting, string excelPath, int batchIndex)
        {
            var image = AppConfigService.Config.ImageSettings["TopPunterCasino"];
            image.ImageName = setting.Exceltitle;
            var imagePath = imageService.ExportImage(excelPath, batchIndex, image.SheetName, image);

            return imagePath;
        }

        private bool SendImage(string imagePath, string group)
        {
            var letstalkGroup = group;
            var letstalkModel = messageService.CreateLetstalkModel(imagePath, letstalkGroup);
            var status = false;
            filesPath.Add(imagePath);
            if (AppConfigService.Config.IsSentDirectly)
            {
                status = letstalkService.SendMessage(letstalkModel);
            }
            else
            {
                status = true;
            }

            return status;
        }
    }
}