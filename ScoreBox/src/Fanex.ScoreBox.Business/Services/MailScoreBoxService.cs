using Fanex.MailDelivery.Client;
using Fanex.MailDelivery.Core.Models;
using RazorEngine;
using RazorEngine.Templating;
using System;
using System.Collections.Generic;
using System.IO;

namespace Fanex.ScoreBox.Business.Services
{
    public interface MailScoreBoxService
    {
        Mail CreateMail(bool isDaily, DateTime fromDate, DateTime toDate, string fileName, bool isNoData);

        void SendMailWhenDataIsNotReady(bool isDaily, DateTime fromDate, DateTime toDate, DateTime lastDate);
    }

    public class MailScoreBoxServiceImp : MailScoreBoxService
    {
        public Mail CreateMail(bool isDaily, DateTime fromDate, DateTime toDate, string fileName, bool isNoData)
        {
            var attachments = new List<MailAttachment>
            {
                new MailAttachment(fileName, FileExtension.MS_EXCEL)
            };

            if (isNoData)
            {
                attachments = new List<MailAttachment>();
            }

            var subject = isDaily
                ? $"Automail Report - Daily Score Box Win Loss Report - {toDate:yyyy/MM/dd}"
                : $"Automail Report - Monthly Score Box Win Loss Report - {toDate:yyyy/MM}";
            var mail = new Mail
            {
                Subject = subject,
                Title = "ONEworks Auto Generated Report Administrator",
                Body = LoadBody(fileName, isNoData, fromDate, toDate),
                Attachments = attachments
            };

            return mail;
        }

        private static string LoadBody(string fileName, bool isNoData, DateTime fromDate, DateTime toDate)
        {
            var shortName = new FileInfo(fileName).Name.Replace(".xlsx", string.Empty);
            var tuple = new Tuple<string, bool, DateTime, DateTime>(shortName, isNoData, fromDate, toDate);
            var template = File.ReadAllText("Templates/NotificationEmail.cshtml");
            var result = Engine.Razor.RunCompile(template, $"template{DateTime.Now.Ticks}", model: tuple);

            return result;
        }

        public void SendMailWhenDataIsNotReady(bool isDaily, DateTime fromDate, DateTime toDate, DateTime lastDate)
        {
            if (AppConfigService.AppConfig.FunctionIdNodata == 0)
            {
                return;
            }

            var body = ReadContentWhenDataIsNotReady(fromDate, toDate, lastDate);
            var subject = isDaily
                ? $"[WARNING] - DATA IS NOT READY - Automail Report - Daily Score Box Win Loss Report - {toDate:yyyy/MM/dd}"
                : $"[WARNING] - DATA IS NOT READY - Automail Report - Monthly Score Box Win Loss Report from {fromDate:yyyy/MM/dd} to {toDate:yyyy/MM/dd}";
            var mail = new Mail
            {
                Subject = subject,
                Title = "ONEworks Auto Generated Report Administrator",
                Body = body,
                Attachments = new List<MailAttachment>(),
                MailingListId = AppConfigService.AppConfig.FunctionIdNodata
            };

            MailClientHandler.Start();
            MailClientHandler.SendFunctionMail(mail);
            MailClientHandler.Stop();
        }

        private static string ReadContentWhenDataIsNotReady(DateTime fromDate, DateTime toDate, DateTime lastDate)
        {
            var tuple = new Tuple<DateTime, DateTime, DateTime>(fromDate, toDate, lastDate);
            var template = File.ReadAllText(@"Templates/ContentWhenDataIsNotReady.cshtml");
            var result = Engine.Razor.RunCompile(template, $"template{DateTime.Now.Ticks}", model: tuple);

            return result;
        }
    }
}