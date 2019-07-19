using Fanex.MailDelivery.Client;
using Fanex.MailDelivery.Core.Models;
using System;

namespace Fanex.FinanceTransfer.Business.Services
{
    public interface MailService
    {
        Mail CreateMail();

        void SendMailToDBA(string content);
    }

    public class MailServiceImp : MailService
    {
        public Mail CreateMail()
        {
            var date = AppConfigService.AppConfig.IsManual ? AppConfigService.AppConfig.TransferMonthDate : DateTime.Today;
            var content = $"Transfer of Super balance for Licensee Company WL for {date:MMM-yyyy} has been completed";

            return new Mail
            {
                Title = "ONEworks Auto Generated Report Administrator",
                Subject = content,
                Body = content
            };
        }

        public void SendMailToDBA(string content)
        {
            var date = AppConfigService.AppConfig.IsManual ? AppConfigService.AppConfig.TransferMonthDate : DateTime.Today;
            var subject = $"Transfer of Super balance for Licensee Company WL for {date:MMM-yyyy} has been failed.";
            var mail = new Mail
            {
                Title = "ONEworks Auto Generated Report Administrator",
                Subject = subject,
                Body = content,
                MailingListId = AppConfigService.AppConfig.MailFunctionId
            };

            MailClientHandler.Start();
            MailClientHandler.SendFunctionMail(mail);
            MailClientHandler.Stop();
        }
    }
}