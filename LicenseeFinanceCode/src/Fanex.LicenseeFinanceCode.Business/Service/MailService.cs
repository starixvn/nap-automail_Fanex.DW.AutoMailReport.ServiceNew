using Fanex.LicenseeFinanceCode.Business.Model;
using Fanex.MailDelivery.Core.Models;
using RazorEngine;
using RazorEngine.Templating;
using System;
using System.Collections.Generic;
using System.IO;

namespace Fanex.LicenseeFinanceCode.Business.Service
{
    public interface MailService
    {
        Mail CreateMail(List<LicenseeTransferCodeModel> rows);
    }

    public class MailServiceImp : MailService
    {
        public Mail CreateMail(List<LicenseeTransferCodeModel> rows)
        {
            return new Mail
            {
                Subject = $"[Licensee Finance Code] Code Needed {DateTime.Today:MM/dd/yyyy}",
                Title = "ONEworks Auto Generated Report Administrator",
                Body = ReadBody(rows),
                Attachments = new List<MailAttachment>()
            };
        }

        private string ReadBody(List<LicenseeTransferCodeModel> rows)
        {
            var tuple = new Tuple<List<LicenseeTransferCodeModel>>(rows);
            var template = File.ReadAllText($"Template/EmailTemplate.cshtml");
            var result = Engine.Razor.RunCompile(template, $"template{DateTime.Now.Ticks}", model: tuple);

            return result;
        }
    }
}