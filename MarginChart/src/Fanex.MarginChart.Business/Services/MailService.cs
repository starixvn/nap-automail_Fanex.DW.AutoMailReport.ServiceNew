using Fanex.MailDelivery.Core.Models;
using RazorEngine;
using RazorEngine.Templating;
using System;
using System.Collections.Generic;
using System.IO;

namespace Fanex.MarginChart.Business.Services
{
    public interface MailService
    {
        Mail SendMail(string fileName);
    }

    public class MailServiceImp : MailService
    {
        private readonly DateTimeServiceProvider dateTimeService;

        public MailServiceImp(DateTimeServiceProvider dateTimeService)
        {
            this.dateTimeService = dateTimeService;
        }

        public Mail SendMail(string fileName)
        {
            var attachments = new List<MailAttachment>
            {
                new MailAttachment(fileName, FileExtension.MS_EXCEL)
            };

            var mail = new Mail
            {
                Title = "ONEworks Auto Generated Report Administrator",
                Subject = $"Automail Report - House Net Income - {dateTimeService.DateTime.ToDate:yyyy/MM/dd}",
                Body = Content(fileName),
                Attachments = attachments
            };

            return mail;
        }

        private string Content(string fileName)
        {
            var tuple = new Tuple<string>(new FileInfo(fileName).Name);
            var template = File.ReadAllText("Templates/DHWL.cshtml");
            var result = Engine.Razor.RunCompile(template, $"template{DateTime.Now.Ticks}", model: tuple);

            return result;
        }
    }
}