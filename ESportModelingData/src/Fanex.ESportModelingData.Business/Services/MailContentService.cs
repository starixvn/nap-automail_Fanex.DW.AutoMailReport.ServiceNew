using RazorEngine;
using RazorEngine.Templating;
using System;
using System.Collections.Generic;
using System.IO;

namespace Fanex.ESportModelingData.Business.Services
{
    public interface MailContentService
    {
        string GenerateMailContent(List<string> fileNames);
    }

    public class MailContentServiceImp : MailContentService
    {
        public string GenerateMailContent(List<string> fileNames)
        {
            var names = new List<string>();
            fileNames.ForEach(n =>
            {
                n = n.Replace(".zip", string.Empty);
                names.Add(n);
            });

            var templateName = fileNames.Count > 0
                ? "Templates/NotificationEmail.cshtml"
                : "Templates/NoData.cshtml";
            var downloadLink = AppConfigService.AppConfig.DownloadLink;
            var tuple = new Tuple<List<string>, string>(names, downloadLink);
            var template = File.ReadAllText(templateName);
            var result = Engine.Razor.RunCompile(template, $"template{DateTime.Now.Ticks}", model: tuple);

            return result;
        }
    }
}