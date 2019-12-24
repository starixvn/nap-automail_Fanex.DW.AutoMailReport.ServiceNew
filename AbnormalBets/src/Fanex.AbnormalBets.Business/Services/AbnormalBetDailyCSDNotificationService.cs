using Fanex.AbnormalBets.Business.Models;
using Fanex.MailDelivery.Core.Models;
using System;

namespace Fanex.AbnormalBets.Business.Services
{
    public interface AbnormalBetDailyCSDNotificationService
    {
        JobMessage Execute();
    }

    public class AbnormalBetDailyCSDNotificationServiceImp : AbnormalBetDailyCSDNotificationService
    {
        private readonly Log log;
        private readonly RepositoryService repositoryService;
        private readonly DateService dateService;
        private readonly TransformService transformService;
        private readonly ExcelDailyCSDService excelService;        

        public AbnormalBetDailyCSDNotificationServiceImp(
            Log log,
            RepositoryService repositoryService,
            DateService dateService,
            TransformService transformService,
            ExcelDailyCSDService excelService)
        {
            this.log = log;
            this.repositoryService = repositoryService;
            this.dateService = dateService;
            this.transformService = transformService;
            this.excelService = excelService;            
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
            var date = dateService.Date();
            var rawData = repositoryService.QueryDailyCSDBet(date);
            var data = transformService.ExecuteDailyCSD(rawData);
            excelService.Execute(data, date);
            return new ExecuteResultModel { Success = true, Mail = null };
        }
    }
}