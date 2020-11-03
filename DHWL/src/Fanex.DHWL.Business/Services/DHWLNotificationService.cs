using Fanex.DHWL.Business.Models;
using Fanex.LetstalkService.Client;
using Fanex.MailDelivery.Core.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Fanex.DHWL.Business.Services
{
    public interface DHWLNotificationService
    {
        JobMessage Execute();
    }

    public class DHWLNotificationServiceImp : DHWLNotificationService
    {
        private readonly Log log;
        private readonly DataService dataService;
        private readonly LetstalkClientService letstalkService;
        private readonly ExcelRawService excelRawService;
        private readonly ExcelDailyService excelDailyService;
        private readonly MessageService messageService;
        private readonly ImageService imageService;
        private readonly FileService fileService;
        private readonly MailService mailService;
        private readonly BaseDataTable baseDataTable;
        private readonly DataHouseIncomeService dataHouseIncomeService;
        private readonly DataLastMonthService dataLastMonthService;
        private readonly ChartExport chartExport;
        private readonly DataMarginService dataMarginService;
        private readonly ExcelMargin excelMargin;

        private List<string> filePaths = new List<string>();
        private Mail mail = new Mail { Subject = string.Empty, Body = string.Empty };

        public DHWLNotificationServiceImp(
            Log log,
            DataService dataService,
            LetstalkClientService letstalkService,
            ExcelRawService excelRawService,
            ExcelDailyService excelDailyService,
            MessageService messageService,
            ImageService imageService,
            FileService fileService,
            MailService mailService,
            BaseDataTable baseDataTable,
            DataHouseIncomeService dataHouseIncomeService,
            DataLastMonthService dataLastMonthService,
            ChartExport chartExport,
            DataMarginService dataMarginService,
            ExcelMargin excelMargin)
        {
            this.log = log;
            this.dataService = dataService;
            this.letstalkService = letstalkService;
            this.excelRawService = excelRawService;
            this.excelDailyService = excelDailyService;
            this.messageService = messageService;
            this.imageService = imageService;
            this.fileService = fileService;
            this.mailService = mailService;
            this.baseDataTable = baseDataTable;
            this.dataHouseIncomeService = dataHouseIncomeService;
            this.dataLastMonthService = dataLastMonthService;
            this.chartExport = chartExport;
            this.dataMarginService = dataMarginService;
            this.excelMargin = excelMargin;
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
            var letstalkFails = new List<string>();
            ProcessHouseIncome();

            var errFiles = new List<string> { };// SendLetstalkImageData(rawData);

            errFiles.ForEach(file =>
            {
                if (file.Length > 1)
                {
                    letstalkFails.Add(file);
                }
            });

            return new ExecuteResultModel
            {
                Success = letstalkFails.Count == 0,
                Message = string.Join(Environment.NewLine, letstalkFails),
                Mail = mail
            };
        }

        private void ProcessHouseIncome()
        {
            var dailyRawData = dataService.QueryDailyData();
            var cloneData = baseDataTable.Clone(dailyRawData);
            var dataForExcel = dataHouseIncomeService.CalTotalByProductForExcel(cloneData);
            var lastMonthData = ProcessLastMonth();
            var dataDailyMargin = dataMarginService.CalcDataForDaily(dailyRawData);
            var dataMTDMargin = dataMarginService.CalcDataForMTD(dailyRawData);

            excelRawService.Export(dailyRawData);
            excelDailyService.Export(dataForExcel, lastMonthData);
            chartExport.Export(dailyRawData);
            excelMargin.Export(dataDailyMargin, true);
            excelMargin.Export(dataMTDMargin, false);
        }

        private DataTable ProcessLastMonth()
        {
            var lastMonthRawData = dataService.QueryLastMonthData();
            return dataLastMonthService.CalcData(lastMonthRawData);
        }

        private void SendMail(string excelAllPath)
        {
        }

        private string ExportImage(string excelPath, int batchIndex, int rowCount)
        {
            var lastRow = rowCount + (rowCount > 0 ? 4 : 5);
            var image = AppConfigService.Config.ImageSettings["DHWLSettings"];
            image.ImageName = "DHWL";
            image.LastRow = lastRow;
            image.CropHeight = lastRow * image.HeightPerRow + 50;
            var imagePath = imageService.ExportImage(excelPath, batchIndex, image.SheetName, image);

            return imagePath;
        }

        private bool SendImage(string imagePath)
        {
            var letstalkGroup = "";
            var letstalkModel = messageService.CreateLetstalkModel(imagePath, letstalkGroup);
            var status = letstalkService.SendMessage(letstalkModel);

            filePaths.Add(imagePath);

            return status;
        }
    }
}