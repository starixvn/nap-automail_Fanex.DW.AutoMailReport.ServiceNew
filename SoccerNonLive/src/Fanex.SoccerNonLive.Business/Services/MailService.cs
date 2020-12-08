using Fanex.MailDelivery.Core.Models;
using RazorEngine;
using RazorEngine.Templating;
using System;
using System.Collections.Generic;
using System.IO;

namespace Fanex.SoccerNonLive.Business.Services
{
    public interface MailService
    {
        Mail SendMail(string fileName, string subject);
    }

    public class MailServiceImp : MailService
    {
        public Mail SendMail(string fileName, string subject)
        {
            var attachments = new List<MailAttachment>
            {
                new MailAttachment(fileName, FileExtension.MS_EXCEL)
            };

            var mail = new Mail
            {
                Title = "ONEworks Auto Generated Report Administrator",
                Subject = $"{subject}",
                Body = Content(fileName),
                Attachments = attachments
            };

            return mail;
        }

        private string Content(string fileName)
        {
            var tuple = new Tuple<string>(new FileInfo(fileName).Name);
            var template = File.ReadAllText("Templates/SoccerNonLive.cshtml");
            var result = Engine.Razor.RunCompile(template, $"template{DateTime.Now.Ticks}", model: tuple);

            return result;
        }
    }
}