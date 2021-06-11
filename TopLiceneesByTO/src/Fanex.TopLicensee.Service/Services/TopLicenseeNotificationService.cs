using Fanex.MailDelivery.Core.Models;
using Fanex.TopLicensee.Service.Models;
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
        private readonly FileServiceImp fileService;

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
            FileServiceImp fileService)
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
            var success = false;

            ExportImageTop50(imageBatches);
            ExportImageTopLicenseeMargin(imageBatches);

            ExportVirtualSoccerHouse(imageBatches, LeagueGroup.VL, SportType.Soccer, "Acc_Rpt_Letstalk_LeagueGroupSport_House");
            // ExportVirtualSoccerHouse(imageBatches, LeagueGroup.SabaSoccerPinGoal, SportType.Soccer, "Acc_Rpt_Letstalk_LeagueGroupSport_House");
            ExportVirtualSoccerHouse(imageBatches, LeagueGroup.VL, SportType.Basketball, "Acc_Rpt_Letstalk_LeagueGroupSport_House");

            ExportVirtualSoccerLicensee(imageBatches, LeagueGroup.VL, SportType.Soccer, "Acc_Rpt_Letstalk_LeagueGroupSport_Licensee");
            // ExportVirtualSoccerLicensee(imageBatches, LeagueGroup.SabaSoccerPinGoal, SportType.Soccer, "Acc_Rpt_Letstalk_LeagueGroupSport_Licensee");

            ExportVirtualSoccerLicensee(imageBatches, LeagueGroup.VL, SportType.Basketball, "Acc_Rpt_Letstalk_LeagueGroupSport_Licensee");

            if (AppConfigService.AppConfig.IsSentDirectly)
            {
                imageBatches.ForEach(files =>
                {
                    success = messageService.Send(files);
                    if (!success)
                    {
                        imageSentFail.AddRange(files);
                    }
                });
            }
            else
            {
                var files = new List<string>();
                imageBatches.ForEach(f =>
                {
                    files.AddRange(f);
                });

                fileService.CopyFiles(files);
                success = true;
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
                    AppConfigService.AppConfig.Image["Top50"].ImageName = sport.SectionName.Replace(" ", string.Empty);
                    var imageFile = imageService.ExportImage(excelFile, sport.SectionName, AppConfigService.AppConfig.Image["Top50"]);
                    imageFiles.Add(imageFile);
                });

                imageBatches.Add(imageFiles);
            }
        }

        private void ExportImageTopLicenseeMargin(List<List<string>> imageBatches)
        {
            var data = dataService.QueryDailyTopLicenseeMargin(true);
            var excelFile = excelTopLicenseeMargin.Export("Daily", data);
            AppConfigService.AppConfig.Image["TopLicenseeMargin"].ImageName = "TopLicenseeMargin_Daily";
            var imageName = imageService.ExportImage(excelFile, "Template", AppConfigService.AppConfig.Image["TopLicenseeMargin"]);
            imageBatches.Add(new List<string> { imageName });

            data = dataService.QueryDailyTopLicenseeMargin(false);
            excelFile = excelTopLicenseeMargin.Export("MTD", data);
            AppConfigService.AppConfig.Image["TopLicenseeMargin"].ImageName = "TopLicenseeMargin_MTD";
            imageName = imageService.ExportImage(excelFile, "Template", AppConfigService.AppConfig.Image["TopLicenseeMargin"]);
            imageBatches.Add(new List<string> { imageName });
        }

        private void ExportVirtualSoccerHouse(List<List<string>> imageBatches, LeagueGroup leagueGroup, SportType sportType, string spName)
        {
            var dataTable = dataService.QueryVirtualSoccer(leagueGroup, sportType, spName);
            var excelPath = exportVirtualSoccerHouse.Export(dataTable, leagueGroup, sportType);

            AppConfigService.AppConfig.Image["VirtualSoccer_House"].ImageName = $"VS_House_{leagueGroup}_{sportType}";
            var imageFile = imageService.ExportImage(excelPath, "Template", AppConfigService.AppConfig.Image["VirtualSoccer_House"]);
            imageBatches.Add(new List<string> { imageFile });
        }

        private void ExportVirtualSoccerLicensee(List<List<string>> imageBatches, LeagueGroup leagueGroup, SportType sportType, string spName)
        {
            var dataTable = dataService.QueryVirtualSoccer(leagueGroup, sportType, spName);
            var excelPath = exportVirtualSoccerLicensee.Export(dataTable, leagueGroup, sportType);

            AppConfigService.AppConfig.Image["VirtualSoccer_Licensee"].ImageName = $"VS_Licensee_{leagueGroup}_{sportType}";
            var imageFile = imageService.ExportImage(excelPath, "Template", AppConfigService.AppConfig.Image["VirtualSoccer_Licensee"]);
            imageBatches.Add(new List<string> { imageFile });
        }
    }
}