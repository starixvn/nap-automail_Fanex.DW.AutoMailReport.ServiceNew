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
        private readonly ConfigSettingModel config;
        private readonly ImageService imageService;
        //private readonly MailService mailService;

        public TopLicenseeNotificationServiceImp(Log log, DataService dataService, ExcelService excelService/*, MailService mailService*/, ImageService imageService)
        {
            this.log = log;
            this.dataService = dataService;
            this.excelService = excelService;
            //this.mailService = mailService;
            this.imageService = imageService;
            config = AppConfigService.AppConfig;
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
                //mailService.SendMailWhenDataIsNotReady();
                return new ExecuteResultModel { Success = false, Message = "DataIsNotReady" };
            }

            var dictData = new Dictionary<string, List<LicenseeTOModel>>();
            var sports = AppConfigService.AppConfig.Sports;
            sports.ForEach(sport => 
            {
                var data = dataService.QueryTopLicensee(sport.SportType);
                dictData.Add(sport.SectionName, data);
            });

            var excelFiles = excelService.Export(dictData);
            imageService.ExportImage(excelFiles, "All Sports", AppConfigService.AppConfig.Image);
            //var mail = mailService.CreateMail(excelFiles);

            return new ExecuteResultModel { Success = true, Mail = null };
        }
    }
}