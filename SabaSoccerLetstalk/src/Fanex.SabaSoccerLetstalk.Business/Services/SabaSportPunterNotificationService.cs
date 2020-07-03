using Fanex.LetstalkService.Client;
using Fanex.MailDelivery.Core.Models;
using Fanex.SabaSoccerLetstalk.Business.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Fanex.SabaSoccerLetstalk.Business.Services
{
    public interface SabaSportPunterNotificationService
    {
        JobMessage Execute();
    }

    public class SabaSportPunterNotificationServiceImp : SabaSportPunterNotificationService
    {
        private readonly Log log;
        private readonly DataService dataService;
        private readonly LetstalkClientService letstalkService;
        private readonly SabaSportPunterExcelService excelService;
        private readonly MessageService messageService;
        private readonly ImageService imageService;
        private readonly SabaSportPunterRefineDataService refineService;
        private readonly FileService fileService;
        private readonly DateRangeService dateService;
        private readonly MailService mailService;

        public SabaSportPunterNotificationServiceImp(
            Log log,
            DataService dataService,
            LetstalkClientService letstalkService,
            SabaSportPunterExcelService excelService,
            MessageService messageService,
            ImageService imageService,
            SabaSportPunterRefineDataService refineService,
            FileService fileService,
            DateRangeService dateService,
            MailService mailService)
        {
            this.log = log;
            this.dataService = dataService;
            this.letstalkService = letstalkService;
            this.excelService = excelService;
            this.messageService = messageService;
            this.imageService = imageService;
            this.refineService = refineService;
            this.fileService = fileService;
            this.dateService = dateService;
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
                jobMessage.Mail = new Mail();
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
            var date = dateService.Today;
            var sports = AppConfigService.Config.SabaSports;
            foreach (var sabaSport in sports)
            {
                var sport = sabaSport.Key;
                var setting = sabaSport.Value;
                var rawData = dataService.QueryDataSabaHourly(date, setting);
                var data = refineService.LoadRefineData(sport, date, rawData);
                refineService.StoreRawData(sport, date, rawData);
                List<string> imageFails;
                if (data.Count > 0)
                {
                    imageFails = SendLetstalkImageData(sport, setting, date, data);
                }
                else
                {
                    imageFails = SendLetstalkImageNoData(sport, setting, date);
                }

                imageFails.ForEach(image =>
                {
                    if (image.Length > 0)
                    {
                        letstalkFails.Add(image);
                    }
                });
            }

            return new ExecuteResultModel
            {
                Success = letstalkFails.Count == 0,
                Mail = new Mail(),
                Message = string.Join(Environment.NewLine, letstalkFails)
            };
        }

        private List<string> SendLetstalkImageData(Sport sport, LetstalkTodayPunterSetting setting, DateTime date, List<PunterModel> data)
        {
            var letstalkFails = new List<string>();
            var rowPerBatch = 50;
            var totalBatch = data.Count / rowPerBatch + ((data.Count % rowPerBatch) > 0 ? 1 : 0);
            var imageSentFail = new List<string>();
            var countRetry = 0;

            for (var batchIndex = 0; batchIndex < totalBatch; batchIndex++)
            {
                var rowsOfBatch = data.Skip(batchIndex * rowPerBatch).Take(rowPerBatch).ToList();
                var excelPath = excelService.ExportExcelWithData(sport, setting.Exceltitle, date, rowsOfBatch, batchIndex, totalBatch, false, setting.Subject);
                var imagePath = ExportImage(sport, excelPath, batchIndex, "SabaSportPunters");
                var status = SendToLetstalk(imagePath, setting.LetstalkGroup);
                countRetry = 0;
                while (!status && countRetry < 10)
                {
                    status = SendToLetstalk(imagePath, setting.LetstalkGroup);
                    countRetry++;
                }

                if (!status)
                {
                    imageSentFail.Add(imagePath);
                }
            }

            var excelAllPath = excelService.ExportExcelWithData(sport, setting.Exceltitle, date, data, 0, 1, true, setting.Subject);
            SendToLetstalk(excelAllPath, setting.LetstalkGroup);
            SendToMail(sport, setting, excelAllPath, date);

            if (imageSentFail.Count > 0)
            {
                letstalkFails.Add($"{string.Join(Environment.NewLine, imageSentFail)} - Letstalk sent images Unsuccessfully.");
            }

            fileService.MoveFiles(AppConfigService.Config.InternalPath, AppConfigService.Config.Path);

            return letstalkFails;
        }

        private void SendToMail(Sport sport, LetstalkTodayPunterSetting setting, string excelAllPath, DateTime date)
        {
            var subject = $"{setting.Subject} - {sport} - Time Range: {AppConfigService.Config.TimeRange}";
            var mail = mailService.CreateMail(excelAllPath, setting.Exceltitle, subject, date);
            mail.MailingListId = setting.MailFunctionId;
            mailService.SendMail(mail);
        }

        private List<string> SendLetstalkImageNoData(Sport sport, LetstalkTodayPunterSetting setting, DateTime date)
        {
            var excelPath = excelService.ExportExcelWithNoData(sport, setting.Exceltitle, date, setting.Subject);
            var imagePath = ExportImage(sport, excelPath, 0, "NoData");
            var status = SendToLetstalk(imagePath, setting.LetstalkGroup);
            SendToMail(sport, setting, excelPath, date);
            return new List<string> { status ? string.Empty : $"{imagePath} - Letstalk sent images Unsuccessfully." };
        }

        private string ExportImage(Sport sport, string excelPath, int batchIndex, string imageSetting)
        {
            var image = AppConfigService.Config.ImageSettings[imageSetting];
            image.ImageName = sport.ToString();
            var imagePath = imageService.ExportImage(excelPath, batchIndex, image.SheetName, image);

            return imagePath;
        }

        private bool SendToLetstalk(string filePath, string letstalkGroup)
        {
            var letstalkModel = messageService.CreateLetstalkModel(filePath, letstalkGroup);
            var status = letstalkService.SendMessage(letstalkModel);
            return status;
        }
    }
}