using Fanex.CN88DailyWL.Business.Models;
using Fanex.LetstalkService.Client;
using Fanex.MailDelivery.Core.Models;
using System;

namespace Fanex.CN88DailyWL.Business.Services
{
    public interface CN88NotifycationService
    {
        JobMessage Execute();
    }

    public class CN88NotifycationServiceImp : CN88NotifycationService
    {
        private readonly Log log;
        private readonly DataService dataService;
        private readonly ExcelService excelService;
        private readonly ImageService imageService;
        private readonly MessageService messageService;
        private readonly LetstalkClientService letstalkService;

        public CN88NotifycationServiceImp(
            Log log, 
            DataService dataService, 
            ExcelService excelService,
            ImageService imageService,
            MessageService messageService, 
            LetstalkClientService letstalkService)
        {
            this.log = log;
            this.dataService = dataService;
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
            var data = dataService.QueryData();
            var excelFile = excelService.Execute(data);
            var fileNames = imageService.ExportImage(excelFile);
            var letstalkModel = messageService.CreateLetstalkModel(fileNames);
            var result = letstalkService.SendMessage(letstalkModel);

            return new ExecuteResultModel { Success = result };
        }
    }
}