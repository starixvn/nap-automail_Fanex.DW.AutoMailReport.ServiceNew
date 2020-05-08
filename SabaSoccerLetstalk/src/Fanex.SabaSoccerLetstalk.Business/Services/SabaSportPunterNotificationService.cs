using Fanex.LetstalkService.Client;
using Fanex.MailDelivery.Core.Models;
using Fanex.SabaSoccerLetstalk.Business.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Fanex.SabaSoccerLetstalk.Business.Services
{
    public interface SabaSportPunterNotificationService
    {
        JobMessage Execute();
    }

    public class SabaSportPunterNotificationServiceImp : SabaSportPunterNotificationService
    {
        private readonly Log log;
        private readonly DataService dataService;
        private readonly LetstalkClientService letstalkService;
        private readonly SabaSportPunterExcelService excelService;
        private readonly MessageService messageService;
        private readonly ImageService imageService;
        private readonly SabaSportPunterRefineDataService refineService;
        private readonly FileService fileService;
        private readonly DateRangeService dateService;

        public SabaSportPunterNotificationServiceImp(
            Log log,
            DataService dataService,
            LetstalkClientService letstalkService,
            SabaSportPunterExcelService excelService,
            MessageService messageService,
            ImageService imageService,
            SabaSportPunterRefineDataService refineService,
            FileService fileService,
            DateRangeService dateService)
        {
            this.log = log;
            this.dataService = dataService;
            this.letstalkService = letstalkService;
            this.excelService = excelService;
            this.messageService = messageService;
            this.imageService = imageService;
            this.refineService = refineService;
            this.fileService = fileService;
            this.dateService = dateService;
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
                return new ExecuteResultModel { Success = false, Message = "DataIsNotReady" };
            }

            var letstalkFails = new List<string>();
            var date = dateService.Today;
            var sports = AppConfigService.Config.SabaSports;
            var dictLeagueGroup = new Dictionary<Sport, string>
            {
                { Sport.Soccer, "42,74" },
                { Sport.Basketball, "42" },
            };
            foreach (var sabaSport in sports)
            {
                var sport = sabaSport.Key;
                var leagueGroup = dictLeagueGroup[sport];
                var rawData = dataService.QueryDataSabaSoccerHourly(date, sabaSport.Value, leagueGroup);
                var data = refineService.LoadRefineData(sport, date, rawData);
                refineService.StoreRawData(sport, date, rawData);
                List<string> imageFails;
                if (data.Count > 0)
                {
                    imageFails = SendLetstalkImageData(sport, date, data);
                }
                else
                {
                    imageFails = SendLetstalkImageNoData(sport, date);
                }

                imageFails.ForEach(image =>
                {
                    if (image.Length > 0)
                    {
                        letstalkFails.Add(image);
                    }
                });
            }

            return new ExecuteResultModel
            {
                Success = letstalkFails.Count == 0,
                Message = string.Join(Environment.NewLine, letstalkFails)
            };
        }

        private List<string> SendLetstalkImageData(Sport sport, DateTime date, List<PunterModel> data)
        {
            var letstalkFails = new List<string>();
            var rowPerBatch = 50;
            var totalBatch = data.Count / rowPerBatch + ((data.Count % rowPerBatch) > 0 ? 1 : 0);
            var imageSentFail = new List<string>();
            var countRetry = 0;

            for (var batchIndex = 0; batchIndex < totalBatch; batchIndex++)
            {
                var rowsOfBatch = data.Skip(batchIndex * rowPerBatch).Take(rowPerBatch).ToList();
                var excelPath = excelService.ExportExcelWithData(sport, date, rowsOfBatch, batchIndex, totalBatch, false);
                var imagePath = ExportImage(sport, excelPath, batchIndex, "SabaSportPunters");
                var status = SendToLetstalk(imagePath);
                countRetry = 0;
                while (!status && countRetry < 10)
                {
                    status = SendToLetstalk(imagePath);
                    countRetry++;
                }

                if (!status)
                {
                    imageSentFail.Add(imagePath);
                }
            }

            var excelAllPath = excelService.ExportExcelWithData(sport, date, data, 0, 1, true);
            SendToLetstalk(excelAllPath);

            if (imageSentFail.Count > 0)
            {
                letstalkFails.Add($"{string.Join(Environment.NewLine, imageSentFail)} - Letstalk sent images Unsuccessfully.");
            }

            fileService.MoveFiles(AppConfigService.Config.InternalPath, AppConfigService.Config.Path);

            return letstalkFails;
        }

        private List<string> SendLetstalkImageNoData(Sport sport, DateTime date)
        {
            var excelPath = excelService.ExportExcelWithNoData(sport, date);
            var imagePath = ExportImage(sport, excelPath, 0, "NoData");
            var status = SendToLetstalk(imagePath);

            return new List<string> { status ? string.Empty : $"{imagePath} - Letstalk sent images Unsuccessfully." };
        }

        private string ExportImage(Sport sport, string excelPath, int batchIndex, string imageSetting)
        {
            var image = AppConfigService.Config.ImageSettings[imageSetting];
            image.ImageName = sport.ToString();
            var imagePath = imageService.ExportImage(excelPath, batchIndex, image.SheetName, image);

            return imagePath;
        }

        private bool SendToLetstalk(string filePath)
        {
            var letstalkModel = messageService.CreateLetstalkModel(filePath);
            var status = letstalkService.SendMessage(letstalkModel);
            return status;
        }
    }
}