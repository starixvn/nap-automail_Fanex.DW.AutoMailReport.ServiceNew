using System;
using Fanex.MailDelivery.Core.Models;
using Fanex.Service.Core;

namespace FastMarketWeeklyPerformance.Business.Services
{
    //public class DistributedNotificationServiceImp : DistributedNotificationService
    //{
    //    private readonly MailService mailService;
    //    private readonly LogService logService;
    //    private readonly RepositoryService repositoryService;

    //    public DistributedNotificationServiceImp(MailService mailService, LogService logService, RepositoryService repositoryService)
    //    {
    //        this.mailService = mailService;
    //        this.logService = logService;
    //        this.repositoryService = repositoryService;
    //    }

    //    public JobMessage Execute()
    //    {
    //        var result = new ExecuteResult();
    //        JobMessage jobMessage;
    //        try
    //        {
    //            result = SendNotification();
    //        }
    //        catch (Exception e)
    //        {
    //            result.Success = false;
    //            result.Message = e.Message;
    //            logService.Log(e);
    //        }
    //        finally
    //        {
    //            jobMessage = new JobMessage
    //            {
    //                Mail = result.Mail,
    //                JobStatus = result.Success ?
    //                JobStatus.Success :
    //                JobStatus.DataIsNotReady,
    //                TaskLog = new TaskLog
    //                {
    //                    LogTime = SystemTime.Now,
    //                    Message = result.Message
    //                }
    //            };
    //        }

    //        return jobMessage;
    //    }

    //    private ExecuteResult SendNotification()
    //    {
    //        if (!repositoryService.IsDataAvailable())
    //        {
    //            mailService.SendMailWhenDataIsNotReady();
    //            return new ExecuteResult { Success = false, Message = "DataIsNotReady" };
    //        }

    //        var mail = mailService.Execute();
    //        return new ExecuteResult { Success = true, Mail = mail };
    //    }
    //}
}