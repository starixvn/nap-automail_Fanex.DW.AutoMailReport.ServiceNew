using Fanex.DHWL.Business.Models;
using Fanex.DHWL.Business.Services.MonthlyDHWLLetstalk;
using Fanex.LetstalkService.Client;
using Fanex.MailDelivery.Core.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Fanex.DHWL.Business.Services
{
    public interface MonthlyDHWLNotificationService
    {
        JobMessage Execute();
    }

    public class MonthlyDHWLNotificationServiceImp : MonthlyDHWLNotificationService
    {
        private readonly Log log;
        private readonly DataService dataService;
        private readonly ExcelRawService excelRawService;
        private readonly MessageService messageService;
        private readonly MailService mailService;
        private readonly FileService fileService;
        private readonly BaseDataTable baseDataTable;
        private readonly MonthlyImageDHWLService imageDHWLService;
        private readonly DateTimeServiceProvider dateTimeServiceProvider;

        public MonthlyDHWLNotificationServiceImp(
            Log log,
            DataService dataService,
            ExcelRawService excelRawService,
            MessageService messageService,
            MailService mailService,
            FileService fileService,
            BaseDataTable baseDataTable,
            MonthlyImageDHWLService imageDHWLService,
            DateTimeServiceProvider dateTimeServiceProvider
            )
        {
            this.log = log;
            this.dataService = dataService;
            this.excelRawService = excelRawService;
            this.messageService = messageService;
            this.mailService = mailService;
            this.fileService = fileService;
            this.baseDataTable = baseDataTable;
            this.imageDHWLService = imageDHWLService;
            this.dateTimeServiceProvider = dateTimeServiceProvider;
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
                var jobSuccess = AppConfigService.Config.IsAttachAdjument
                    ? JobStatus.Success
                    : JobStatus.SuccessWithoutSendmail;

                jobMessage.Mail = result.Mail;
                jobMessage.JobStatus = result.Success ? jobSuccess : JobStatus.DataIsNotReady;
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
            var monthlyRawData = dataService.QueryDHWLData();
            var dataReconciliation = dataService.QueryReconciliation();
            var cloneData = baseDataTable.Clone(monthlyRawData);
            var fromdate = dateTimeServiceProvider.DateTime.FromDate;
            var todate = dateTimeServiceProvider.DateTime.ToDate;

            excelRawService.Export(monthlyRawData, false, fromdate, todate);
            var images = imageDHWLService.Export(monthlyRawData, dataReconciliation);

            CopyFiles(images);
            var status = messageService.SendMessage(images, AppConfigService.Config.LetstalkGroup);
            var errFile = status.Where(s => !s.Value).Select(s => s.Key).ToList();

            var subject = $"Automail Report - Monthly HWL Report and Licensee Rebate & Discount Data Soft Copy Files - {todate:yyyy/MM/dd}";
            var excelPath = imageDHWLService.FileName;
            var mail = mailService.SendMail(excelPath, subject);

            return new ExecuteResultModel
            {
                Mail = mail,
                Message = string.Join(Environment.NewLine, errFile),
                Success = errFile.Count == 0
            };
        }

        private void CopyFiles(List<string> images)
        {
            foreach (var group in AppConfigService.Config.LetstalkGroups)
            {
                fileService.CopyFiles(group.Value, images);
            }
        }
    }
}