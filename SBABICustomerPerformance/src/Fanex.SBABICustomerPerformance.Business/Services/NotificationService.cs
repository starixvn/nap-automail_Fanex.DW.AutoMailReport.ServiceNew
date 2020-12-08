using Fanex.LetstalkService.Client;
using Fanex.MailDelivery.Core.Models;
using Fanex.SBABICustomerPerformance.Business.Models;
using System;
using System.Collections.Generic;

namespace Fanex.SBABICustomerPerformance.Business.Services
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
        private readonly ExcelRawService excelRawService;
        private readonly MessageService messageService;
        private readonly DateTimeServiceProvider dateService;

        private readonly FileService fileService;
        private readonly MailService mailService;

        private List<string> filePaths = new List<string>();

        public NotificationServiceImp(
            Log log,
            DataService dataService,
            LetstalkClientService letstalkService,
            ExcelRawService excelRawService,
            MessageService messageService,

            FileService fileService,
            MailService mailService,
            DateTimeServiceProvider dateService
        )
        {
            this.log = log;
            this.dataService = dataService;
            this.letstalkService = letstalkService;
            this.excelRawService = excelRawService;
            this.messageService = messageService;

            this.fileService = fileService;
            this.mailService = mailService;
            this.dateService = dateService;
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
            var rawData = dataService.QueryRawData();
            var excelPath = excelRawService.ExportExcel(rawData);
            var mail = SendMail(excelPath);

            return new ExecuteResultModel
            {
                Success = true,
                Message = string.Empty,
                Mail = mail
            };
        }

        private Mail SendMail(string excelAllPath)
        {
            var date = dateService.DateTime.WinlostDate;
            var subject = $"SB Danger Level ABI 50 Customer Performance - {date:yyyy/MM/dd}";
            return mailService.SendMail(excelAllPath, subject, subject, date);
        }
    }
}