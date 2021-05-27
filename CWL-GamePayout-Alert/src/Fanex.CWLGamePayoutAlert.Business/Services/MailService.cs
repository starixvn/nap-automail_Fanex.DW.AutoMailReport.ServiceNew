using Fanex.CWLGamePayoutAlert.Business.Models;
using Fanex.MailDelivery.Client;
using Fanex.MailDelivery.Core.Models;
using RazorEngine;
using RazorEngine.Templating;
using System;
using System.Collections.Generic;
using System.IO;

namespace Fanex.CWLGamePayoutAlert.Business.Services
{
    public interface MailService
    {
        void SendMail(DateTime fromGMT8Time, List<PayoutModel> data);
    }

    public class MailServiceImp : MailService
    {
        public void SendMail(DateTime fromGMT8Time, List<PayoutModel> data)
        {
            var attachments = new List<MailAttachment> { };

            var mail = new Mail
            {
                Title = "ONEworks Auto Generated Report Administrator",
                Subject = $"Non-sport Products - Payout Limit Alert - {fromGMT8Time:MM/dd/yyyy HH:00} - {fromGMT8Time:HH:59} (GMT+8)",
                Body = Content(fromGMT8Time, data),
                Attachments = attachments,
                MailingListId = data.Count > 0 ? AppConfigService.Config.MailId : AppConfigService.Config.NoDataMailId
            };

            MailClientHandler.Start();
            MailClientHandler.SendFunctionMail(mail);
            MailClientHandler.Stop();
        }

        private string Content(DateTime fromGMT8Time, List<PayoutModel> data)
        {
            var tuple = new Tuple<DateTime, List<PayoutModel>>(fromGMT8Time, data);
            var template = data.Count > 0
                ? File.ReadAllText("Templates/CWLGamePayoutAlert.cshtml")
                : File.ReadAllText("Templates/CWLGamePayoutAlert-NoData.cshtml");
            var result = Engine.Razor.RunCompile(template, $"template{DateTime.Now.Ticks}", model: tuple);

            return result;
        }
    }
}