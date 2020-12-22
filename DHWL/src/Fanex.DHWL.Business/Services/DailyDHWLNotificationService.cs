using Fanex.DHWL.Business.Models;
using Fanex.DHWL.Business.Services.DailyDHWLLetstalk;
using Fanex.DHWL.Business.Services.Margin;
using Fanex.LetstalkService.Client;
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
        private readonly LetstalkClientService letstalkService;
        private readonly ExcelRawService excelRawService;
        private readonly DailyExcelService excelDailyService;
        private readonly MessageService messageService;
        private readonly FileService fileService;
        private readonly MailService mailService;
        private readonly BaseDataTable baseDataTable;
        private readonly DataHouseIncomeService dataHouseIncomeService;
        private readonly ChartExport chartExport;
        private readonly DailyImageDHWLService imageDHWLService;
        private readonly ImageMarginService imageMarginService;

        private List<string> filePaths = new List<string>();
        private Mail mail = new Mail { Subject = string.Empty, Body = string.Empty };

        public DailyDHWLNotificationServiceImp(
            Log log,
            DataService dataService,
            LetstalkClientService letstalkService,
            ExcelRawService excelRawService,
            DailyExcelService excelDailyService,
            MessageService messageService,
            FileService fileService,
            MailService mailService,
            BaseDataTable baseDataTable,
            DataHouseIncomeService dataHouseIncomeService,
            DataLastMonthService dataLastMonthService,
            ChartExport chartExport,
            DailyImageDHWLService imageDHWLService,
            ImageMarginService imageMarginService
            )
        {
            this.log = log;
            this.dataService = dataService;
            this.letstalkService = letstalkService;
            this.excelRawService = excelRawService;
            this.excelDailyService = excelDailyService;
            this.messageService = messageService;
            this.fileService = fileService;
            this.mailService = mailService;
            this.baseDataTable = baseDataTable;
            this.dataHouseIncomeService = dataHouseIncomeService;
            this.chartExport = chartExport;
            this.imageDHWLService = imageDHWLService;
            this.imageMarginService = imageMarginService;
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
            var dailyRawData = dataService.QueryDHWLData();
            var cloneData = baseDataTable.Clone(dailyRawData);

            excelRawService.Export(dailyRawData, true);
            var excelPath = excelDailyService.Export(cloneData);

            var imageDHWLPath = imageDHWLService.Export(dailyRawData);
            var imageMarginPath = imageMarginService.Export(dailyRawData, true);
            var imageMarginMTDPath = imageMarginService.Export(dailyRawData, false);
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
            var mail = mailService.SendMail(excelPath);

            return new ExecuteResultModel
            {
                Mail = mail,
                Message = string.Join(Environment.NewLine, errFile),
                Success = errFile.Count == 0
            };
        }

        private void CopyFiles(string dhwlExcelPath, List<string> images)
        {
            foreach (var group in AppConfigService.Config.LetstalkGroups)
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