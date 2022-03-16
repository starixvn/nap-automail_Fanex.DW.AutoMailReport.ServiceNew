using Fanex.DHWL.Business.Models;
using Fanex.DHWL.Business.Services.DailyDHWLLetstalk;
using Fanex.DHWL.Business.Services.Margin;
using Fanex.MailDelivery.Core.Models;
using System;
using System.Collections.Generic;
using System.Data;
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

        private readonly ChartExport chartExport;

        private readonly DailyImageDHWLService imageDHWLService;
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
            ChartExport chartExport,
            DailyImageDHWLService imageDHWLService,
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
            this.imageDHWLService = imageDHWLService;
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
            var checkData = dataService.CheckDataAvailable();
            if (checkData.Count > 0)
            {
                return new ExecuteResultModel
                {
                    Mail = new Mail(),
                    Message = string.Join(",", checkData),
                    Success = false
                };
            }

            var dailyRawData = dataService.QueryDHWLData();
            var cloneData = baseDataTable.Clone(dailyRawData);
            var cloneDataDHWL = baseDataTable.Clone(dailyRawData);
            var fromdate = dateTimeServiceProvider.DateTime.FromDate;
            var todate = dateTimeServiceProvider.DateTime.ToDate;

            excelRawService.Export(dailyRawData, "Daily", fromdate, todate);
            var excelPath = excelDailyService.Export(cloneData);

            var reportId = 1;
            var imageDHWLPath = imageDHWLService.Export(cloneDataDHWL, reportId);

            reportId++;
            var imageMarginPath = imageMarginService.Export(dailyRawData, true, reportId);

            reportId++;
            var imageMarginMTDPath = imageMarginService.Export(dailyRawData, false, reportId);
            var chartPath = chartExport.Export(dailyRawData);
            var images = new List<string>
            {
                imageDHWLPath,
                imageMarginPath,
                imageMarginMTDPath,
                chartPath
            };

            CopyFiles(excelPath, images);
            var status = messageService.SendMessage(images, AppConfigService.Config.LetstalkGroup);

            var errFile = status.Where(s => !s.Value).Select(s => s.Key).ToList();
            var subject = $"Automail Report - House Net Income - {todate:yyyy/MM/dd}";
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