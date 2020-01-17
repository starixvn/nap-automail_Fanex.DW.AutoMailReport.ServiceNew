using Fanex.MailDelivery.Core.Models;
using Fanex.TopLicensee.Service.Models;
using System;
using System.Collections.Generic;

namespace Fanex.TopLicensee.Service.Services
{
    public interface TopLicenseeNotificationService
    {
        JobMessage Execute();
    }

    public class TopLicenseeNotificationServiceImp : TopLicenseeNotificationService
    {
        private readonly Log log;
        private readonly DataService dataService;
        private readonly ExcelService excelService;        
        private readonly ImageService imageService;
        private readonly MessageService messageService;
        private readonly MailService mailService;

        public TopLicenseeNotificationServiceImp(
            Log log,
            DataService dataService,
            ExcelService excelService, 
            MailService mailService,
            ImageService imageService,
            MessageService messageService)
        {
            this.log = log;
            this.dataService = dataService;
            this.excelService = excelService;
            this.mailService = mailService;
            this.imageService = imageService;
            this.messageService = messageService;            
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
                mailService.SendMailWhenDataIsNotReady();
                return new ExecuteResultModel { Success = false, Message = "DataIsNotReady" };
            }

            var result = SendMessage();
            var message = string.Empty;
            if (!result)
            {
                message = "Letsalk sent images Unsuccessfully.";
            }

            return new ExecuteResultModel { Success = result, Mail = null, Message = message };
        }

        private bool SendMessage()
        {
            var batches = AppConfigService.AppConfig.Sports;
            var imageBatches = new List<List<string>>();
            var success = true;
            foreach (var batch in batches)
            {
                var imageFiles = new List<string>();
                var dictData = new Dictionary<string, List<LicenseeTOModel>>();
                batch.Value.ForEach(sport =>
                {
                    var data = dataService.QueryTopLicensee(sport.SportType);
                    dictData.Add(sport.SectionName, data);
                });

                var excelFile = excelService.Export(batch.Key, dictData);

                batch.Value.ForEach(sport =>
                {
                    AppConfigService.AppConfig.Image.ImageName = sport.SectionName.Replace(" ", string.Empty);
                    var imageFile = imageService.ExportImage(excelFile, sport.SectionName, AppConfigService.AppConfig.Image);
                    imageFiles.Add(imageFile);
                });

                imageBatches.Add(imageFiles);
            }

            imageBatches.ForEach(files =>
            {
                var result = messageService.Send(files);
                if (!result)
                {
                    success = result;
                }
            });


            return success;
        }
    }
}