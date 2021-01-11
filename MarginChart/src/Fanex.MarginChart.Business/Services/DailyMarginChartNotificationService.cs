using Fanex.LetstalkService.Client;
using Fanex.MailDelivery.Core.Models;
using Fanex.MarginChart.Business.Models;
using Fanex.MarginChart.Business.Services.DailyDHWLLetstalk;
using Fanex.MarginChart.Business.Services.Margin;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Fanex.MarginChart.Business.Services
{
    public interface DailyMarginChartNotificationService
    {
        JobMessage Execute();
    }

    public class DailyMarginChartNotificationServiceImp : DailyMarginChartNotificationService
    {
        private readonly Log log;
        private readonly DataService dataService;
        private readonly LetstalkClientService letstalkService;
        private readonly ExcelRawService excelRawService;
        private readonly MessageService messageService;
        private readonly FileService fileService;
        private readonly MailService mailService;
        private readonly BaseDataTable baseDataTable;
        private readonly DataHouseIncomeService dataHouseIncomeService;
        private readonly ChartExport chartExport;
        private readonly DailyImageDHWLService imageDHWLService;
        private readonly ImageMarginService imageMarginService;
        private readonly DateTimeServiceProvider dateTimeServiceProvider;
        private readonly DataMarginService dataMarginService;
        private readonly ExcelMarginService excelMarginService;
        private readonly ChartService chartService;

        private List<string> filePaths = new List<string>();

        public DailyMarginChartNotificationServiceImp(
            Log log,
            DataService dataService,
            LetstalkClientService letstalkService,
            ExcelRawService excelRawService,
            MessageService messageService,
            FileService fileService,
            MailService mailService,
            BaseDataTable baseDataTable,
            DataHouseIncomeService dataHouseIncomeService,
            DataLastMonthService dataLastMonthService,
            ChartExport chartExport,
            DailyImageDHWLService imageDHWLService,
            ImageMarginService imageMarginService,
            DateTimeServiceProvider dateTimeServiceProvider,
            DataMarginService dataMarginService,
            ExcelMarginService excelMarginService,
            ChartService chartService)
        {
            this.log = log;
            this.dataService = dataService;
            this.letstalkService = letstalkService;
            this.excelRawService = excelRawService;
            this.messageService = messageService;
            this.fileService = fileService;
            this.mailService = mailService;
            this.baseDataTable = baseDataTable;
            this.dataHouseIncomeService = dataHouseIncomeService;
            this.chartExport = chartExport;
            this.imageDHWLService = imageDHWLService;
            this.imageMarginService = imageMarginService;
            this.dateTimeServiceProvider = dateTimeServiceProvider;
            this.dataMarginService = dataMarginService;
            this.excelMarginService = excelMarginService;
            this.chartService = chartService;
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
            var dailyRawData = dataService.QueryMarginChartData();
            var source = baseDataTable.CloneWithoutIgnoreFigureRows(dailyRawData);
            var fromdate = dateTimeServiceProvider.DateTime.FromDate;
            var todate = dateTimeServiceProvider.DateTime.ToDate;
            var products = AppConfigService.Config.ProductGroups;
            var yesterdayColumnIndex = source.Columns.Count - 1;
            var mtdColumnIndex = 10;

            excelRawService.Export(dailyRawData, true, fromdate, todate);

            products.ForEach(product =>
            {
                var category = product.ProductCategory;

                var margins = ExportMargin(source, yesterdayColumnIndex, mtdColumnIndex, category);
                var charts = ExportChart(product, source, category);
            });

            var images = new List<string>
            {
            };

            //CopyFiles(excelPath, images);
            var status = messageService.SendMessage(images, AppConfigService.Config.LetstalkGroup);
            var errFile = status.Where(s => !s.Value).Select(s => s.Key).ToList();

            return new ExecuteResultModel
            {
                Mail = new Mail(),
                Message = string.Join(Environment.NewLine, errFile),
                Success = errFile.Count == 0
            };
        }

        private List<string> ExportMargin(DataTable source, int yesterdayColumnIndex, int mtdColumnIndex, string category)
        {
            var dailyData = dataMarginService.CalcDataByCategory(source, category, yesterdayColumnIndex);
            var mtdData = dataMarginService.CalcDataByCategory(source, category, mtdColumnIndex);
            var excelDaily = excelMarginService.Export(dailyData, true, category);
            var excelMtd = excelMarginService.Export(mtdData, false, category);
            var imgDaily = imageMarginService.Export(excelDaily, true, category, excelMarginService.LastRowIndex);
            var imgMTD = imageMarginService.Export(excelMtd, false, category, excelMarginService.LastRowIndex);

            return new List<string>
            {
                imgDaily,
                imgMTD
            };
        }

        private List<string> ExportChart(ProductGroup product, DataTable source, string category)
        {
            var dataWLForCreditChart = dataMarginService.CalcWinLossForChart(category, source, DataType.Credit);
            var dataTOForCreditChart = dataMarginService.CalcTurnoverForChart(category, source, DataType.Credit);
            var creditChart = chartService.ExportDailyChart(product, "Credit", dataWLForCreditChart, dataTOForCreditChart);

            var dataWLForLicenseeChart = dataMarginService.CalcWinLossForChart(category, source, DataType.Licensee);
            var dataTOForLicenseeChart = dataMarginService.CalcTurnoverForChart(category, source, DataType.Licensee);
            var licenseeChart = chartService.ExportDailyChart(product, "Licensee", dataWLForLicenseeChart, dataTOForLicenseeChart);

            var dataWLForAllChart = dataMarginService.CalcWinLossForChart(category, source, DataType.All);
            var dataTOForAllChart = dataMarginService.CalcTurnoverForChart(category, source, DataType.All);
            var allChart = chartService.ExportDailyChart(product, "All", dataWLForAllChart, dataTOForAllChart);

            return new List<string>
            {
                creditChart,
                licenseeChart,
                allChart
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