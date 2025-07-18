using Fanex.DHWL.Business.Models;
using Fanex.DHWL.Business.Services.MonthlyChart;
using Fanex.MailDelivery.Core.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Fanex.DHWL.Business.Services
{
    public interface MonthlyChartNotificationService
    {
        JobMessage Execute();
    }

    public class MonthlyChartNotificationServiceImp : MonthlyChartNotificationService
    {
        private readonly Log log;
        private readonly DataService dataService;

        private readonly MessageService messageService;
        private readonly FileService fileService;

        private readonly MonthlyChartExport chartExport;
        private readonly BaseDataTable baseDataTable;

        public MonthlyChartNotificationServiceImp(
            Log log,
            DataService dataService,
            MessageService messageService,
            FileService fileService,
            MonthlyChartExport chartExport,
            BaseDataTable baseDataTable)
        {
            this.log = log;
            this.dataService = dataService;
            this.messageService = messageService;
            this.fileService = fileService;
            this.chartExport = chartExport;
            this.baseDataTable = baseDataTable;
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
            var checkData = dataService.CheckDataReadyDaily();
            if (checkData.Count > 0)
            {
                var message = string.Join(",", checkData);
                log.Log(new Exception($"Letstalk - DHWL - YTD: {message}"));
                return new ExecuteResultModel
                {
                    Mail = new Mail { Subject = "Subject", Body = "Body", Title = "Title" },
                    Message = message,
                    Success = false
                };
            }

            var monthlyRawData = dataService.QueryDhwlMonthlyChart();

            //var json = JsonConvert.SerializeObject(monthlyRawData, Formatting.Indented);
            //File.WriteAllText($"{AppConfigService.Config.Path}/RawData-Monthly-Chart-05.json", json);
            //var json = File.ReadAllText("Templates/RawData-Monthly-Chart-05.json");
            //var monthlyRawData = JsonConvert.DeserializeObject<DataTable>(json);
            var monthlyRawDataScaled = baseDataTable.ThousandScale(monthlyRawData, 11);

            var chartPathCreditOnly = chartExport.Export(monthlyRawDataScaled, DepositType.CreditOnly);
            var chartPathDepositOnly = chartExport.Export(monthlyRawDataScaled, DepositType.DepositOnly);
            var chartPathAll = chartExport.Export(monthlyRawDataScaled, DepositType.All);
            var images = new List<string>
            {
                chartPathCreditOnly,
                chartPathDepositOnly,
                chartPathAll,
            };

            CopyFiles(images);
            var status = messageService.SendMessage(images, AppConfigService.Config.LetstalkGroup);

            var errFile = status.Where(s => !s.Value).Select(s => s.Key).ToList();

            return new ExecuteResultModel
            {
                Mail = new Mail() { Subject = string.Empty, Body = string.Empty },
                Message = string.Join(Environment.NewLine, errFile),
                Success = errFile.Count == 0
            };
        }

        private void CopyFiles(List<string> images)
        {
            var groups = AppConfigService.Config.LetstalkGroups;
            foreach (var group in groups)
            {
                fileService.CopyFiles(group.Value, images);
            }
        }
    }
}