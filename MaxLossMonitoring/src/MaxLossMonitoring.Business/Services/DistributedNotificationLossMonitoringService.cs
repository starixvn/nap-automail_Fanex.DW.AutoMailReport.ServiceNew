using System;
using Fanex.MailDelivery.Core.Models;
using Fanex.Service.Core;

namespace MaxLossMonitoring.Business.Services
{
    public interface DistributedNotificationLossMonitoringService
    {
        JobMessage Execute();
    }

    public class DistributedNotificationLossMonitoringServiceImp : DistributedNotificationLossMonitoringService
    {
        private readonly DataService dataService;
        private readonly LogService logService;
        private readonly MessageService messageService;
        private readonly MailServiceLossMonitoringService mailService;

        public DistributedNotificationLossMonitoringServiceImp(DataService dataService, LogService logService, MessageService messageService, MailServiceLossMonitoringService mailService)
        {
            this.dataService = dataService;
            this.logService = logService;
            this.messageService = messageService;
            this.mailService = mailService;
        }

        public JobMessage Execute()
        {
            var result = new ExecuteResult();
            JobMessage jobMessage;
            try
            {
                result = SendNotification();
            }
            catch (Exception e)
            {
                result.Success = false;
                result.Message = e.Message;
                logService.Log(e);
            }
            finally
            {
                jobMessage = new JobMessage
                {
                    Mail = result.Mail,
                    JobStatus = result.Success ?
                    JobStatus.Success :
                    JobStatus.DataIsNotReady,
                    TaskLog = new TaskLog
                    {
                        LogTime = DateTime.Now,
                        Message = result.Message
                    }
                };
            }

            return jobMessage;
        }

        private ExecuteResult SendNotification()
        {
            var dataSourceForLetstalk = dataService.QueryMonitorListForLetstalk();
            var dataSourceForEmail = dataService.QueryMonitorListForEmail();
            var mail = mailService.Execute(dataSourceForEmail);
            var success = messageService.SendMessage(dataSourceForLetstalk);
            return new ExecuteResult { Success = success, Mail = mail };
        }
    }
}