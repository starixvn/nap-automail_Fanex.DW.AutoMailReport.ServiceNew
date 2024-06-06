using Fanex.DHWL.Business.Models;
using Fanex.DHWL.Business.Services.MonthlyDHWLLetstalk;
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
        private readonly MonthlyImageDHWLService imageDhwlService;
        private readonly DateTimeServiceProvider dateTimeServiceProvider;

        public MonthlyDHWLNotificationServiceImp(
            Log log,
            DataService dataService,
            ExcelRawService excelRawService,
            MessageService messageService,
            MailService mailService,
            FileService fileService,
            BaseDataTable baseDataTable,
            MonthlyImageDHWLService imageDhwlService,
            DateTimeServiceProvider dateTimeServiceProvider)
        {
            this.log = log;
            this.dataService = dataService;
            this.excelRawService = excelRawService;
            this.messageService = messageService;
            this.mailService = mailService;
            this.fileService = fileService;
            this.baseDataTable = baseDataTable;
            this.imageDhwlService = imageDhwlService;
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
            var checkData = dataService.CheckDataReadyMonthly();
            if (checkData.Count > 0)
            {
                return new ExecuteResultModel
                {
                    Mail = new Mail(),
                    Message = string.Join(",", checkData),
                    Success = false
                };
            }

            var monthlyRawData = dataService.QueryDhwlData();

            //var json = JsonConvert.SerializeObject(monthlyRawData, Formatting.Indented);
            //File.WriteAllText($"{AppConfigService.Config.Path}/RawData-Monthly-05.json", json);
            //var json = File.ReadAllText("Templates/RawData-Monthly-05.json");
            //monthlyRawData = JsonConvert.DeserializeObject<DataTable>(json);

            var dataSetReconciliation = dataService.QueryReconciliation();
            //var jsonReconciliation = JsonConvert.SerializeObject(dataSetReconciliation, Formatting.Indented);
            //File.WriteAllText($"{AppConfigService.Config.Path}/RawData-Monthly-Reconciliation-05.json", jsonReconciliation);
            //var jsonReconciliation = File.ReadAllText("Templates/RawData-Monthly-Reconciliation-05.json");
            //var dataSetReconciliation = JsonConvert.DeserializeObject<DataSet>(jsonReconciliation);

            var dataReconciliation = dataSetReconciliation.Tables[0];
            var dataBa = dataSetReconciliation.Tables[1];

            var fromDate = dateTimeServiceProvider.DateTime.FromDate;
            var toDate = dateTimeServiceProvider.DateTime.ToDate;

            var reportId = 1;
            excelRawService.Export(monthlyRawData, "Monthly", fromDate, toDate);
            var images = imageDhwlService.Export(monthlyRawData, dataReconciliation, dataBa, reportId);

            CopyFiles(images);
            var status = messageService.SendMessage(images, AppConfigService.Config.LetstalkGroup);
            var errFile = status.Where(s => !s.Value).Select(s => s.Key).ToList();

            var subject = $"Automail Report - Monthly HWL Report and Licensee Rebate & Discount Data Soft Copy Files - {toDate:yyyy/MM/dd}";
            var excelPath = imageDhwlService.FileName;
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