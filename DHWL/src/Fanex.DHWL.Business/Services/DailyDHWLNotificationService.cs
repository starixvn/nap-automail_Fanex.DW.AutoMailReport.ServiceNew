using Fanex.DHWL.Business.Models;
using Fanex.DHWL.Business.Services.DailyChart;
using Fanex.DHWL.Business.Services.DailyDHWLLetstalk;
using Fanex.DHWL.Business.Services.HouseNetIncome;
using Fanex.DHWL.Business.Services.Margin;
using Fanex.DHWL.Business.Services.Webhook;
using Fanex.MailDelivery.Core.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;

namespace Fanex.DHWL.Business.Services
{
    public interface DailyDHWLNotificationService
    {
        JobMessage Execute();
    }

    public class DailyDHWLNotificationServiceImp : DailyDHWLNotificationService
    {
        private readonly Log log;
        private readonly DataService dataService;

        private readonly ExcelRawService excelRawService;

        private readonly DailyExcelService excelDailyService;
        private readonly MessageService messageService;
        private readonly FileService fileService;
        private readonly MailService mailService;
        private readonly BaseDataTable baseDataTable;

        private readonly DailyChartExport chartExport;

        private readonly DailyImageDHWLService imageDhwlService;
        private readonly ImageMarginService imageMarginService;
        private readonly ImageMarginHedgingSpecialService imageMarginHedgingSpecialService;
        private readonly DateTimeServiceProvider dateTimeServiceProvider;
        private readonly WebhookDataService webhookDataService;

        public DailyDHWLNotificationServiceImp(
            Log log,
            DataService dataService,
            ExcelRawService excelRawService,
            DailyExcelService excelDailyService,
            MessageService messageService,
            FileService fileService,
            MailService mailService,
            BaseDataTable baseDataTable,
            DailyChartExport chartExport,
            DailyImageDHWLService imageDhwlService,
            ImageMarginService imageMarginService,
            ImageMarginHedgingSpecialService imageMarginHedgingSpecialService,
            DateTimeServiceProvider dateTimeServiceProvider,
            WebhookDataService webhookDataService)
        {
            this.log = log;
            this.dataService = dataService;
            this.excelRawService = excelRawService;
            this.excelDailyService = excelDailyService;
            this.messageService = messageService;
            this.fileService = fileService;
            this.mailService = mailService;
            this.baseDataTable = baseDataTable;
            this.chartExport = chartExport;
            this.imageDhwlService = imageDhwlService;
            this.imageMarginService = imageMarginService;
            this.imageMarginHedgingSpecialService = imageMarginHedgingSpecialService;
            this.dateTimeServiceProvider = dateTimeServiceProvider;
            this.webhookDataService = webhookDataService;
        }

        public JobMessage Execute()
        {
            log.Log("Daily DHWL Notification Service started at " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

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

                var path = $"{AppConfigService.Config.Path}/{DateTime.Today:yyyyMMdd}";
                File.WriteAllText($"{path}/DHWL-ErrLog.txt", $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - {e.StackTrace}");
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

                var path = $"{AppConfigService.Config.Path}/{DateTime.Today:yyyyMMdd}";
                File.WriteAllText($"{path}/DHWL-Log.txt", $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - {result.Message}");
            }

            return jobMessage;
        }

        private ExecuteResultModel SendNotification()
        {
            var checkData = dataService.CheckDataReadyDaily();
            if (checkData.Count > 0)
            {
                var message = string.Join(Environment.NewLine, checkData);
                log.Log(new Exception($"Letstalk - DHWL - Daily - PRE: {message}"));
                return new ExecuteResultModel
                {
                    Mail = new Mail { Subject = "Subject", Body = "Body", Title = "Title" },
                    Message = message,
                    Success = false
                };
            }

            var dailyRawFullData = dataService.QueryDhwlData();

            //var json = JsonConvert.SerializeObject(dailyRawFullData, Formatting.Indented);
            //File.WriteAllText($"{AppConfigService.Config.Path}/DailyRawData.json", json);
            //var json = File.ReadAllText("Templates/RawData-16.json");
            //var dailyRawFullData = JsonConvert.DeserializeObject<DataTable>(json);

            var dailyRawData = baseDataTable.ThousandScale(dailyRawFullData, 10);
            var cloneData = baseDataTable.Clone(dailyRawFullData);
            var cloneDataDhwl = baseDataTable.Clone(dailyRawData);
            var fromDate = dateTimeServiceProvider.DateTime.FromDate;
            var toDate = dateTimeServiceProvider.DateTime.ToDate;

            webhookDataService.Execute(toDate, dailyRawData);

            var previousRawData = baseDataTable.ThousandScale(dataService.QueryDhwlDataPreviousYear(), 10);
            var previousRawDataDeposit = previousRawData.AsEnumerable().Where(s => Convert.ToBoolean(s["IsDeposit"]))
                .CopyToDataTable();
            var previousRawDataCredit = previousRawData.AsEnumerable().Where(s => !Convert.ToBoolean(s["IsDeposit"]))
                .CopyToDataTable();
            var dailyRawDataDeposit =
                dailyRawData.AsEnumerable().Any(s => Convert.ToBoolean(s["IsDeposit"]))
                    ? dailyRawData.AsEnumerable().Where(s => Convert.ToBoolean(s["IsDeposit"])).CopyToDataTable()
                    : dailyRawData.Clone();
            var dailyRawDataCredit = dailyRawData.AsEnumerable().Any(s => !Convert.ToBoolean(s["IsDeposit"]))
                ? dailyRawData.AsEnumerable().Where(s => !Convert.ToBoolean(s["IsDeposit"])).CopyToDataTable()
                : dailyRawData.Clone();

            excelRawService.Export(dailyRawData, "Daily", fromDate, toDate);
            var excelPath = excelDailyService.Export(cloneData);

            var reportId = 1;

            reportId++;
            var imageMarginPath = imageMarginService.Export(dailyRawData, true, reportId);
            var grandTotalForDaily = imageMarginService.GrandTotalList;

            reportId++;
            var imageMarginMtdPath = imageMarginService.Export(dailyRawData, false, reportId);
            var grandTotalForMtd = imageMarginService.GrandTotalList;

            reportId++;
            var imageMarginHedgingSpecialPath = imageMarginHedgingSpecialService.Export(dailyRawData, true, reportId);
            var grandTotalHedgingSpecialForDaily = imageMarginHedgingSpecialService.GrandTotalList;

            reportId++;
            var imageMarginMtdHedgingSpecialPath =
                imageMarginHedgingSpecialService.Export(dailyRawData, false, reportId);
            var grandTotalForHedgingSpecialMtd = imageMarginHedgingSpecialService.GrandTotalList;

            var imageDhwlPath = imageDhwlService.Export(cloneDataDhwl, grandTotalForDaily, grandTotalForMtd);
            var chartPath = chartExport.Export(dailyRawData, previousRawData, "DHWL", "DHWL-Chart");
            var chartPathCredit = chartExport.Export(dailyRawDataCredit, previousRawDataCredit, "CREDIT DHWL",
                "CREDIT-DHWL-Chart");
            var chartPathDeposit = chartExport.Export(dailyRawDataDeposit, previousRawDataDeposit, "LICENSEE DHWL",
                "LICENSEE-DHWL-Chart");
            var images = new List<string>
            {
                imageDhwlPath,
                imageMarginPath,
                imageMarginMtdPath,
                imageMarginHedgingSpecialPath,
                imageMarginMtdHedgingSpecialPath,
                chartPath,
                chartPathCredit,
                chartPathDeposit
            };

            CopyFiles(excelPath, images);
            var status = messageService.SendMessage(images, AppConfigService.Config.LetstalkGroup);

            var errFile = status.Where(s => !s.Value).Select(s => s.Key).ToList();
            var subject = $"Automail Report - House Net Income - {toDate:yyyy/MM/dd}";
            var mail = mailService.SendMail(excelPath, subject);

            return new ExecuteResultModel
            {
                Mail = mail,
                Message = string.Join(Environment.NewLine, errFile),
                Success = errFile.Count == 0
            };
        }

        private void CopyFiles(string dhwlExcelPath, List<string> images)
        {
            var groups = AppConfigService.Config.LetstalkGroups;
            foreach (var group in groups)
            {
                if (group.Key.Contains("Excel"))
                {
                    fileService.CopyFiles(group.Value, new List<string> { dhwlExcelPath });
                }
                else
                {
                    fileService.CopyFiles(group.Value, images);
                }
            }
        }
    }
}