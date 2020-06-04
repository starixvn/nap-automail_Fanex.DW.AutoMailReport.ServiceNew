using Fanex.LetstalkService.Client;
using Fanex.MailDelivery.Core.Models;
using Fanex.N88DailyWL.Business.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Fanex.N88DailyWL.Business.Services
{
    public interface N88NotificationService
    {
        JobMessage Execute();
    }

    public class N88NotificationServiceImp : N88NotificationService
    {
        private readonly Log log;
        private readonly DataService dataService;
        private readonly DateService dateService;
        private readonly ExcelService excelService;
        private readonly ImageService imageService;
        private readonly MessageService messageService;
        private readonly LetstalkClientService letstalkService;

        public N88NotificationServiceImp(
            Log log,
            DataService dataService,
            DateService dateService,
            ExcelService excelService,
            ImageService imageService,
            MessageService messageService,
            LetstalkClientService letstalkService)
        {
            this.log = log;
            this.dataService = dataService;
            this.dateService = dateService;
            this.excelService = excelService;
            this.imageService = imageService;
            this.messageService = messageService;
            this.letstalkService = letstalkService;
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
            if (!dateService.IsDataAvailable())
            {
                return new ExecuteResultModel { Success = false, Message = "DataIsNotReady" };
            }

            var data = dataService.QueryData();
            var excelPath = excelService.ExportExcel(data);
            var imageN88Path = imageService.ExportImage(excelPath, "Template", AppConfigService.AppConfig.N88);
            var imageDepositPath = imageService.ExportImage(excelPath, "Template", AppConfigService.AppConfig.Deposit);
            var letstalkModel = messageService.CreateLetstalkModel(new List<string> { imageN88Path, imageDepositPath });
            var result = new List<bool>();
            letstalkModel.ForEach(model =>
            {
                var status = letstalkService.SendMessage(model);
                result.Add(status);
            });

            var sendResult = !result.Any(c => !c);
            var message = sendResult ? string.Empty : "Letstalk sends images unsuccessfully.";
            return new ExecuteResultModel { Success = sendResult, Message = message };
        }
    }
}