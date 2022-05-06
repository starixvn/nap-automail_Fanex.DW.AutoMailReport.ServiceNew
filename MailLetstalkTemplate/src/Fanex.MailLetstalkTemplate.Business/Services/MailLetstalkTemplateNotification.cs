using Fanex.LetstalkService.Client;
using Fanex.MailDelivery.Core.Models;
using Fanex.MailLetstalkTemplate.Business.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Fanex.MailLetstalkTemplate.Business.Services
{
    public interface MailLetstalkTemplateNotification
    {
        JobMessage Execute();
    }

    public class MailLetstalkTemplateNotificationImp : MailLetstalkTemplateNotification
    {
        private readonly Log log;
        private readonly DataService dataService;
        private readonly LetstalkClientService letstalkService;
        private readonly ExcelService excelService;
        private readonly MailService mailService;
        private readonly MessageService messageService;
        private readonly ImageService imageService;
        private readonly CsvService csvService;

        public MailLetstalkTemplateNotificationImp(
              Log log,
            DataService dataService,
            LetstalkClientService letstalkService,
            ExcelService excelService,
            MailService mailService,
            MessageService messageService,
            ImageService imageService,
            CsvService csvService)
        {
            this.log = log;
            this.dataService = dataService;
            this.letstalkService = letstalkService;
            this.excelService = excelService;
            this.mailService = mailService;
            this.messageService = messageService;
            this.imageService = imageService;
            this.csvService = csvService;
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
                var lastDate = dataService.LastDateDW();
                return new ExecuteResultModel
                {
                    Success = false,
                    Message = $"DataIsNotReady, Last Date from DW is {lastDate.LastDate:yyyy/MM/dd}"
                };
            }

            var letstalkFails = new List<string>();
            var reposts = AppConfigService
                .Reports
                .Where(r => r.IsActive)
                .ToList();

            var date = DateTime.Today;
            var mails = dataService.QueryMail(date);
            foreach (var report in reposts)
            {
                var text = File.ReadAllText($"Templates/{report.ReportFile}");
                var config = JsonConvert.DeserializeObject<ConfigSettingModel>(text);
                AppConfigService.Config = config;

                var isSent = mailService.IsMailSent(mails, config.MailName);
                if (isSent)
                {
                    continue;
                }

                var jsonData = "Data Not Begin Yet";
                var jsonExcelPath = "Excel Not Begin Yet";
                var filePath = string.Empty;
                try
                {
                    var data = dataService.QueryData(config.SPName);
                    filePath = excelService.ExportExcel(data);

                    jsonData = "Reading data ...";
                    jsonExcelPath = JsonConvert.SerializeObject(filePath, Formatting.Indented);

                    if (report.IsEmailEnabled)
                    {
                        mailService.SendMail(filePath);
                    }

                    if (report.IsLetstalkEnabled)
                    {
                        var imagePaths = ExportImage(config, filePath);
                        var sentLetstalkSuccess = SendImage(imagePaths);
                        if (!sentLetstalkSuccess)
                        {
                            letstalkFails.Add($"{report.ReportFile} - Letstalk sent images Unsuccessfully.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    var err = $"Report: {report.ReportFile}, err: {ex.Message}, data: {jsonData}, excel: {jsonExcelPath}";
                    letstalkFails.Add(err);
                    log.Log(err);
                }
            }

            return new ExecuteResultModel
            {
                Success = letstalkFails.Count == 0,
                Message = string.Join(Environment.NewLine, letstalkFails)
            };
        }

        private List<string> ExportImage(ConfigSettingModel config, string excelPath)
        {
            var imagePaths = new List<string>();
            foreach (var image in config.ImageSettings)
            {
                var imagePath = imageService.ExportImage(excelPath, image.Value.SheetName, image.Value);
                imagePaths.Add(imagePath);
            }

            return imagePaths;
        }

        private bool SendImage(List<string> imagePaths)
        {
            var letstalkModel = messageService.CreateLetstalkModel(imagePaths);
            return letstalkService.SendMessage(letstalkModel);
        }
    }
}