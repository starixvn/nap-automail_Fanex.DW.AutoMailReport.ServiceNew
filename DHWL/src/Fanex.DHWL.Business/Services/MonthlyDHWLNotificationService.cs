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
        private readonly LetstalkClientService letstalkService;
        private readonly ExcelRawService excelRawService;
        private readonly MessageService messageService;
        private readonly FileService fileService;
        private readonly BaseDataTable baseDataTable;
        private readonly MonthlyImageDHWLService imageDHWLService;
        private readonly DateTimeServiceProvider dateTimeServiceProvider;

        private List<string> filePaths = new List<string>();
        private Mail mail = new Mail { Subject = string.Empty, Body = string.Empty };

        public MonthlyDHWLNotificationServiceImp(
            Log log,
            DataService dataService,
            LetstalkClientService letstalkService,
            ExcelRawService excelRawService,
            MessageService messageService,
            FileService fileService,
            BaseDataTable baseDataTable,
            DataLastMonthService dataLastMonthService,
            MonthlyImageDHWLService imageDHWLService,
            DateTimeServiceProvider dateTimeServiceProvider
            )
        {
            this.log = log;
            this.dataService = dataService;
            this.letstalkService = letstalkService;
            this.excelRawService = excelRawService;
            this.messageService = messageService;
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
            var monthlyRawData = dataService.QueryDHWLData();
            var cloneData = baseDataTable.Clone(monthlyRawData);
            var fromdate = dateTimeServiceProvider.DateTime.FromDate;
            var todate = dateTimeServiceProvider.DateTime.ToDate;

            excelRawService.Export(monthlyRawData, false, fromdate, todate);
            var imageDHWLPath = imageDHWLService.Export(monthlyRawData);
            var images = new List<string>
            {
                imageDHWLPath,
            };

            CopyFiles(images);
            var status = messageService.SendMessage(images, AppConfigService.Config.LetstalkGroup);
            var errFile = status.Where(s => !s.Value).Select(s => s.Key).ToList();

            return new ExecuteResultModel
            {
                Mail = new Mail(),
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