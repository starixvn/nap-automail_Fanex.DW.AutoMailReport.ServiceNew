using Fanex.MailDelivery.Core.Models;
using Fanex.TopLicensee.Service.Models;
using Fanex.TopLicenseePerformance.Business.Services;
using System;
using System.Collections.Generic;

namespace Fanex.TopLicensee.Service.Services
{
    public interface TopLicenseeNotificationService
    {
        JobMessage Execute();
    }

    public class TopLicenseeNotificationServiceImp : TopLicenseeNotificationService
    {
        private readonly Log log;
        private readonly DataService dataService;
        private readonly ExcelTop50Service excelService;
        private readonly ExcelTopLicenseeMarginService excelTopLicenseeMargin;
        private readonly ExportVirtualSoccerHouse exportVirtualSoccerHouse;
        private readonly ExportVirtualSoccerLicensee exportVirtualSoccerLicensee;
        private readonly ImageService imageService;
        private readonly MessageService messageService;
        private readonly MailService mailService;
        private readonly FileService fileService;

        public TopLicenseeNotificationServiceImp(
            Log log,
            DataService dataService,
            ExcelTop50Service excelService,
            ExcelTopLicenseeMarginService excelTopLicenseeMargin,
            ExportVirtualSoccerHouse exportVirtualSoccerHouse,
            ExportVirtualSoccerLicensee exportVirtualSoccerLicensee,
            MailService mailService,
            ImageService imageService,
            MessageService messageService,
            FileService fileService)
        {
            this.log = log;
            this.dataService = dataService;
            this.excelService = excelService;
            this.excelTopLicenseeMargin = excelTopLicenseeMargin;
            this.exportVirtualSoccerHouse = exportVirtualSoccerHouse;
            this.exportVirtualSoccerLicensee = exportVirtualSoccerLicensee;
            this.mailService = mailService;
            this.imageService = imageService;
            this.messageService = messageService;
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
            if (!dataService.IsDataAvailable())
            {
                mailService.SendMailWhenDataIsNotReady();
                return new ExecuteResultModel { Success = false, Message = "DataIsNotReady" };
            }

            var imageSentFail = new List<string>();
            var result = SendMessage(imageSentFail);
            var message = string.Empty;

            if (!result)
            {
                message = $"Letsalk sent images Unsuccessfully.{Environment.NewLine} {string.Join(Environment.NewLine, imageSentFail)}";
            }

            return new ExecuteResultModel { Success = result, Mail = new Mail(), Message = message };
        }

        private bool SendMessage(List<string> imageSentFail)
        {
            var imageBatches = new List<List<string>>();
            var success = true;

            ExportVirtualSoccerLicensee(imageBatches, LeagueGroup.VL, SportType.Soccer);
            ExportVirtualSoccerLicensee(imageBatches, LeagueGroup.VL, SportType.Basketball);

            ExportImageTop50(imageBatches);
            ExportImageTopLicenseeMargin(imageBatches);

            if (AppConfigService.AppConfig.IsSentDirectly)
            {
                imageBatches.ForEach(files =>
                {
                    var result = messageService.SendToLicenseeVirtualGroup(files);
                    if (!result)
                    {
                        success = result;
                        imageSentFail.AddRange(files);
                    }
                });
            }
            else
            {
                var images = new List<string>();
                imageBatches.ForEach(files =>
                {
                    files.ForEach(file =>
                    {
                        images.Add(file);
                    });
                });

                fileService.CopyFiles(images);
            }

            return success;
        }

        private void ExportImageTop50(List<List<string>> imageBatches)
        {
            var batches = AppConfigService.AppConfig.Sports;
            foreach (var batch in batches)
            {
                var imageFiles = new List<string>();
                var dictData = new Dictionary<string, List<LicenseeTOModel>>();
                batch.Value.ForEach(sport =>
                {
                    var data = dataService.QueryTopLicensee(sport.SportType);
                    dictData.Add(sport.SectionName, data);
                });

                var excelFile = excelService.Export(batch.Key, dictData);

                batch.Value.ForEach(sport =>
                {
                    var image = AppConfigService.AppConfig.Image["Top50"];
                    var lastRow = excelService.LastRowIndex;
                    image.ImageName = sport.SectionName.Replace(" ", string.Empty);
                    image.LastRow = lastRow;
                    image.CropHeight = lastRow * image.HeightPerRow + image.AdjustHeight;

                    var imageFile = imageService.ExportImage(excelFile, sport.SectionName, image);
                    imageFiles.Add(imageFile);
                });

                imageBatches.Add(imageFiles);
            }
        }

        private void ExportImageTopLicenseeMargin(List<List<string>> imageBatches)
        {
            var data = dataService.QueryDailyTopLicenseeMargin(true);
            var excelFile = excelTopLicenseeMargin.Export("Daily", data);
            var image = AppConfigService.AppConfig.Image["TopLicenseeMargin"];
            image.ImageName = "TopLicenseeMargin_Daily";
            var lastRow = excelTopLicenseeMargin.LastRowIndex;
            image.LastRow = lastRow;
            image.CropHeight = lastRow * image.HeightPerRow + image.AdjustHeight;

            var imageName = imageService.ExportImage(excelFile, "Template", image);
            imageBatches.Add(new List<string> { imageName });

            data = dataService.QueryDailyTopLicenseeMargin(false);
            excelFile = excelTopLicenseeMargin.Export("MTD", data);
            image = AppConfigService.AppConfig.Image["TopLicenseeMargin"];
            image.ImageName = "TopLicenseeMargin_MTD";
            lastRow = excelTopLicenseeMargin.LastRowIndex;
            image.LastRow = lastRow;
            image.CropHeight = lastRow * image.HeightPerRow + image.AdjustHeight;

            imageName = imageService.ExportImage(excelFile, "Template", image);
            imageBatches.Add(new List<string> { imageName });
        }

        private void ExportVirtualSoccerLicensee(List<List<string>> imageBatches, LeagueGroup leagueGroup, SportType sportType)
        {
            var data = dataService.QueryVirtualSoccer(leagueGroup, sportType);
            var excelPath = exportVirtualSoccerLicensee.Export(data, leagueGroup, sportType);

            var image = AppConfigService.AppConfig.Image["VirtualSoccer_Licensee"];
            image.ImageName = $"VS_Licensee_{leagueGroup}_{sportType}";
            var lastRow = exportVirtualSoccerLicensee.LastRowIndex;
            image.LastRow = lastRow;
            image.CropHeight = lastRow * image.HeightPerRow + image.AdjustHeight;

            var imageFile = imageService.ExportImage(excelPath, "Template", image);
            imageBatches.Add(new List<string> { imageFile });
        }
    }
}