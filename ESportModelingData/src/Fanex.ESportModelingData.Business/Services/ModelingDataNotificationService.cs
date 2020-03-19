using Fanex.ESportModelingData.Business.Models;
using Fanex.MailDelivery.Core.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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
        private readonly ParamService paramService;

        public ModelingDataNotificationServiceImp(Log log, RepositoryService repositoryService, ExportService exportService, MailService mailService, ParamService paramService)
        {
            this.log = log;
            this.repositoryService = repositoryService;
            this.exportService = exportService;
            this.mailService = mailService;
            this.paramService = paramService;
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
            var dateList = paramService.Dates();
            var minDate = dateList.FirstOrDefault().DateStart;
            var maxDate = dateList.LastOrDefault().DateEnd;
            var shortNames = new List<string>();
            foreach (var date in dateList)
            {
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

            var mail = mailService.CreateMail(minDate, maxDate, shortNames);

            return new ExecuteResultModel { Success = true, Mail = mail };
        }
    }
}