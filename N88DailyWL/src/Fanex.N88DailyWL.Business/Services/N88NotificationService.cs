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
            if (!dataService.IsDataAvailable())
            {
                return new ExecuteResultModel { Success = false, Message = "DataIsNotReady" };
            }

            var n88DataYesterday = dataService.QueryN88DataYesterday();
            var n88DataMTD = dataService.QueryN88DataMTD();
            var groups = AppConfigService.AppConfig.LicenseeNameGroups;
            var result = new List<bool>();
            var allImages = new List<string>();

            var currencyYesterday = n88DataYesterday.Item1;
            var currencyMTD = n88DataMTD.Item1;
            var productYesterday = n88DataYesterday.Item2;
            var productMTD = n88DataMTD.Item2;

            foreach (var group in groups)
            {
                if (group.LicenseeName.Equals("All"))
                {
                    Send(result, group, allImages);
                }
                else
                {
                    SendDataByLicensee(result, allImages, currencyYesterday, currencyMTD, productYesterday, productMTD, group);
                }
            }

            var sendResult = !result.Any(c => !c);
            var message = sendResult ? string.Empty : "Letstalk sends images unsuccessfully.";
            return new ExecuteResultModel { Success = sendResult, Message = message };
        }

        private void SendDataByLicensee(
            List<bool> result,
            List<string> allImages,
            List<N88CurrencyModel> currencyYesterday,
            List<N88CurrencyModel> currencyMTD,
            List<N88ProductModel> productYesterday,
            List<N88ProductModel> productMTD,
            LicenseeNameGroup group)
        {
            var currencyYesterdayByLicensee = currencyYesterday.Where(s => s.LicenseeName == group.LicenseeName).ToList();
            var currencyMTDByLicensee = currencyMTD.Where(s => s.LicenseeName == group.LicenseeName).ToList();
            var productYesterdayByLicensee = productYesterday.Where(s => s.LicenseeName == group.LicenseeName).ToList();
            var productMTDByLicensee = productMTD.Where(s => s.LicenseeName == group.LicenseeName).ToList();

            var excelCurrencyYesterday = excelService.ExportCurrency(group.LicenseeName, currencyYesterdayByLicensee, true);
            var excelCurrencyMTD = excelService.ExportCurrency(group.LicenseeName, currencyMTDByLicensee, false);
            var excelProductYesterday = excelService.ExportProduct(group.LicenseeName, productYesterdayByLicensee, true);
            var excelProductMTD = excelService.ExportProduct(group.LicenseeName, productMTDByLicensee, false);

            var imgCurrency = ExportImageCurrency(group.LicenseeName, currencyYesterdayByLicensee, excelCurrencyYesterday, true);
            var imgCurrencyMTD = ExportImageCurrency(group.LicenseeName, currencyMTDByLicensee, excelCurrencyMTD, false);
            var imgProduct = ExportImageProduct(group.LicenseeName, productYesterdayByLicensee, excelProductYesterday, true);
            var imgProductMTD = ExportImageProduct(group.LicenseeName, productMTDByLicensee, excelProductMTD, false);
            var imgs = new List<string> { imgCurrency, imgCurrencyMTD, imgProduct, imgProductMTD };
            allImages.AddRange(imgs);
            Send(result, group, imgs);
        }

        private void Send(List<bool> result, LicenseeNameGroup group, List<string> imgs)
        {
            if (AppConfigService.AppConfig.IsSentDirectly)
            {
                var f = SendImage(imgs, group.Group);
                result.AddRange(f);
            }
            else
            {
                fileService.CopyFiles(imgs, group.Group);
            }
        }

        private List<bool> SendImage(List<string> imgs, string group)
        {
            var result = new List<bool>();
            imgs.ForEach(img =>
            {
                var letstalkModel = messageService.CreateLetstalkModel(img, group);
                var status = letstalkService.SendMessage(letstalkModel);
                result.Add(status);
            });

            return result;
        }

        private string ExportImageCurrency(string licenseeName, List<N88CurrencyModel> data, string excelCurrencyPath, bool isYesterday)
        {
            var imageProperty = AppConfigService.AppConfig.Currency;
            var lastRow = imageProperty.FirstRow + data.Count + (data.Count > 0 ? 1 : 2);
            var height = lastRow * imageProperty.HeightPerRow;
            imageProperty.CropHeight = height;
            imageProperty.LastRow = lastRow;
            imageProperty.ImageName = isYesterday ? $"{licenseeName}-Currency" : $"{licenseeName}-Currency-MTD";
            return imageService.ExportImage(excelCurrencyPath, "Currency", imageProperty);
        }

        private string ExportImageProduct(string licenseeName, List<N88ProductModel> data, string excelProductPath, bool isYesterday)
        {
            var imageProperty = AppConfigService.AppConfig.Product;
            var lastRow = imageProperty.FirstRow + 1 + data.Count;
            var height = lastRow * imageProperty.HeightPerRow;
            imageProperty.CropHeight = height;
            imageProperty.LastRow = lastRow;
            imageProperty.ImageName = isYesterday ? $"{licenseeName}-Product" : $"{licenseeName}-Product-MTD";
            return imageService.ExportImage(excelProductPath, "Product", imageProperty);
        }
    }
}