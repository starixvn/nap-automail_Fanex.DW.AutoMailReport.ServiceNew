using Fanex.LetstalkService.Client;
using Fanex.MailDelivery.Core.Models;
using Fanex.SabaSoccerLetstalk.Business.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Fanex.SabaSoccerLetstalk.Business.Services
{
    public interface SabaSoccerNotificationService
    {
        JobMessage Execute();
    }

    public class SabaSoccerNotificationServiceImp : SabaSoccerNotificationService
    {
        private readonly Log log;
        private readonly DataService dataService;
        private readonly LetstalkClientService letstalkService;
        private readonly ExcelService excelService;
        private readonly MessageService messageService;
        private readonly ImageService imageService;

        public SabaSoccerNotificationServiceImp(
              Log log,
            DataService dataService,
            LetstalkClientService letstalkService,
            ExcelService excelService,
            MessageService messageService,
            ImageService imageService)
        {
            this.log = log;
            this.dataService = dataService;
            this.letstalkService = letstalkService;
            this.excelService = excelService;
            this.messageService = messageService;
            this.imageService = imageService;
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
            var data = dataService.QueryData();
            var rowPerBatch = 50;
            var totalBatch = data.Count / rowPerBatch + ((data.Count % rowPerBatch) > 0 ? 1 : 0);
            var imageSentFail = new List<string>();
            var countRetry = 0;

            for (var batchIndex = 0; batchIndex < totalBatch; batchIndex++)
            {
                var rowsOfBatch = data.Skip(batchIndex * rowPerBatch).Take(rowPerBatch).ToList();
                var excelPath = excelService.ExportExcel(rowsOfBatch, batchIndex, totalBatch);
                var imagePath = ExportImage(excelPath, batchIndex);
                var status = SendImage(imagePath);
                countRetry = 0;
                while (!status && countRetry < 10)
                {
                    status = SendImage(imagePath);
                    countRetry++;
                }

                if (!status)
                {
                    imageSentFail.Add(imagePath);
                }
            }

            if (imageSentFail.Count > 0)
            {
                letstalkFails.Add($"{string.Join(Environment.NewLine, imageSentFail)} - Letstalk sent images Unsuccessfully.");
            }

            return new ExecuteResultModel
            {
                Success = letstalkFails.Count == 0,
                Message = string.Join(Environment.NewLine, letstalkFails)
            };
        }

        private string ExportImage(string excelPath, int batchIndex)
        {
            var image = AppConfigService.Config.ImageSettings["SabaSoccer"];
            var imagePath = imageService.ExportImage(excelPath, batchIndex, image.SheetName, image);

            return imagePath;
        }

        private bool SendImage(string imagePath)
        {
            var letstalkModel = messageService.CreateLetstalkModel(imagePath);
            var status = letstalkService.SendMessage(letstalkModel);
            return status;
        }
    }
}