using Fanex.ESportModelingData.Business.Models;
using Fanex.MailDelivery.Core.Models;
using System;
using System.Collections.Generic;
using System.IO;

namespace Fanex.ESportModelingData.Business.Services
{
    public interface ModelingDataNotificationService
    {
        JobMessage Execute();
    }

    public class ModelingDataNotificationServiceImp : ModelingDataNotificationService
    {
        private readonly Log log;
        private readonly RepositoryService repositoryService;
        private readonly ExportService exportService;
        private readonly MailService mailService;
        private readonly DateRangeService daterange;

        public ModelingDataNotificationServiceImp(
            Log log,
            RepositoryService repositoryService,
            ExportService exportService,
            MailService mailService,
            DateRangeService daterange)
        {
            this.log = log;
            this.repositoryService = repositoryService;
            this.exportService = exportService;
            this.mailService = mailService;
            this.daterange = daterange;
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
            var dates = daterange.Dates(AppConfigService.AppConfig.Period);
            var shortNames = new List<string>();

            foreach (var date in dates)
            {
                var textLog = $"Date: {date.DateEnd}, Sport: {AppConfigService.AppConfig.Sport}, Period: {AppConfigService.AppConfig.Period} {Environment.NewLine}";
                log.Log(textLog);

                AppConfigService.DateStart = date.DateStart;
                var rawData = repositoryService.Query(date.DateStart, date.DateEnd);
                var custInfo = repositoryService.QueryCustInfo(date.DateStart, date.DateEnd);

                if (rawData.Count == 0)
                {
                    continue;
                }

                exportService.ExportCustInfo(date.DateStart, date.DateEnd, custInfo);
                var fileName = exportService.Execute(date.DateStart, date.DateEnd, rawData);
                var shortName = new FileInfo(fileName).Name;
                shortNames.Add(shortName);
            }

            var mail = mailService.CreateMail(shortNames);
            return new ExecuteResultModel { Success = true, Mail = mail };
        }
    }
}