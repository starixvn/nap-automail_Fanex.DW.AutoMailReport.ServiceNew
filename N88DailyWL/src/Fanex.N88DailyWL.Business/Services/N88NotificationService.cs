using Fanex.LetstalkService.Client;
using Fanex.MailDelivery.Core.Models;
using Fanex.N88DailyWL.Business.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Fanex.N88DailyWL.Business.Services
{
    public interface N88NotificationService
    {
        JobMessage Execute();
    }

    public class N88NotificationServiceImp : N88NotificationService
    {
        private readonly Log log;
        private readonly DataService dataService;
        private readonly DateService dateService;
        private readonly ExcelService excelService;
        private readonly ImageService imageService;
        private readonly MessageService messageService;
        private readonly LetstalkClientService letstalkService;
        private readonly FileService fileService;

        public N88NotificationServiceImp(
            Log log,
            DataService dataService,
            DateService dateService,
            ExcelService excelService,
            ImageService imageService,
            MessageService messageService,
            LetstalkClientService letstalkService,
            FileService fileService)
        {
            this.log = log;
            this.dataService = dataService;
            this.dateService = dateService;
            this.excelService = excelService;
            this.imageService = imageService;
            this.messageService = messageService;
            this.letstalkService = letstalkService;
            this.fileService = fileService;
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
                jobMessage.Mail = new Mail();
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
            if (!dateService.IsDataAvailable())
            {
                return new ExecuteResultModel { Success = false, Message = "DataIsNotReady" };
            }

            var n88DataYesterday = dataService.QueryN88DataYesterday();
            var n88DataMTD = dataService.QueryN88DataMTD();

            var currencyYesterday = n88DataYesterday.Item1;
            var currencyMTD = n88DataMTD.Item1;
            var productYesterday = n88DataYesterday.Item2;
            var productMTD = n88DataMTD.Item2;

            var excelCurrencyYesterday = excelService.ExportCurrency(currencyYesterday, true);
            var excelCurrencyMTD = excelService.ExportCurrency(currencyMTD, false);
            var excelProductYesterday = excelService.ExportProduct(productYesterday, true);
            var excelProductMTD = excelService.ExportProduct(productMTD, false);

            var imgCurrency = ExportImageCurrency(currencyYesterday, excelCurrencyYesterday, true);
            var imgCurrencyMTD = ExportImageCurrency(currencyMTD, excelCurrencyMTD, false);
            var imgProduct = ExportImageProduct(productYesterday, excelProductYesterday, true);
            var imgProductMTD = ExportImageProduct(productMTD, excelProductMTD, false);
            var imgs = new List<string>
            {
                imgCurrency,
                imgCurrencyMTD,
                imgProduct,
                imgProductMTD
            };

            var result = new List<bool>();
            if (AppConfigService.AppConfig.IsSentDirectly)
            {
                result = SendImage(imgs);
            }
            else
            {
                AppConfigService.AppConfig.Group.ForEach(group =>
                {
                    fileService.CopyFiles(imgs, group);
                });
            }

            var sendResult = !result.Any(c => !c);
            var message = sendResult ? string.Empty : "Letstalk sends images unsuccessfully.";
            return new ExecuteResultModel { Success = sendResult, Message = message };
        }

        private List<bool> SendImage(List<string> imgs)
        {
            var letstalkModel = messageService.CreateLetstalkModel(imgs);
            var result = new List<bool>();
            letstalkModel.ForEach(model =>
            {
                var status = letstalkService.SendMessage(model);
                result.Add(status);
            });
            return result;
        }

        private string ExportImageCurrency(List<N88CurrencyModel> data, string excelCurrencyPath, bool isYesterday)
        {
            var imageProperty = AppConfigService.AppConfig.Currency;
            var lastRow = imageProperty.FirstRow + data.Count + (data.Count > 0 ? 1 : 2);
            var height = lastRow * imageProperty.HeightPerRow;
            imageProperty.CropHeight = height;
            imageProperty.LastRow = lastRow;
            imageProperty.ImageName = isYesterday ? "N88-Currency" : "N88-Currency-MTD";
            return imageService.ExportImage(excelCurrencyPath, "Currency", imageProperty);
        }

        private string ExportImageProduct(List<N88ProductModel> data, string excelProductPath, bool isYesterday)
        {
            var imageProperty = AppConfigService.AppConfig.Product;
            var lastRow = imageProperty.FirstRow + 1 + data.Count;
            var height = lastRow * imageProperty.HeightPerRow;
            imageProperty.CropHeight = height;
            imageProperty.LastRow = lastRow;
            imageProperty.ImageName = isYesterday ? "N88-Product" : "N88-Product-MTD";
            return imageService.ExportImage(excelProductPath, "Product", imageProperty);
        }
    }
}