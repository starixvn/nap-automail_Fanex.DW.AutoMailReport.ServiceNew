using Fanex.DHWL.Business.Models;
using Fanex.DHWL.Business.Services.DailyChart;
using Fanex.DHWL.Business.Services.DailyDHWLLetstalk;
using Fanex.DHWL.Business.Services.Margin;
using Fanex.MailDelivery.Core.Models;
using System;
using System.Collections.Generic;
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
        private readonly DateTimeServiceProvider dateTimeServiceProvider;

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
            DateTimeServiceProvider dateTimeServiceProvider)
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
            var checkData = dataService.CheckDataReadyDaily();
            if (checkData.Count > 0)
            {
                return new ExecuteResultModel
                {
                    Mail = new Mail(),
                    Message = string.Join(",", checkData),
                    Success = false
                };
            }

            var dailyRawData = dataService.QueryDhwlData();
            var cloneData = baseDataTable.Clone(dailyRawData);
            var cloneDataDhwl = baseDataTable.Clone(dailyRawData);
            var fromDate = dateTimeServiceProvider.DateTime.FromDate;
            var toDate = dateTimeServiceProvider.DateTime.ToDate;
            var previousRawData = dataService.QueryDhwlDataPreviousYear();

            excelRawService.Export(dailyRawData, "Daily", fromDate, toDate);
            var excelPath = excelDailyService.Export(cloneData);

            var reportId = 1;

            reportId++;
            var imageMarginPath = imageMarginService.Export(dailyRawData, true, reportId);
            var grandTotalForDaily = imageMarginService.GrandTotal;

            reportId++;
            var imageMarginMtdPath = imageMarginService.Export(dailyRawData, false, reportId);
            var grandTotalForMtd = imageMarginService.GrandTotal;

            var imageDhwlPath = imageDhwlService.Export(cloneDataDhwl, grandTotalForDaily, grandTotalForMtd);
            var chartPath = chartExport.Export(dailyRawData, previousRawData);
            var images = new List<string>
            {
                imageDhwlPath,
                imageMarginPath,
                imageMarginMtdPath,
                chartPath
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