using System;
using System.Collections.Generic;
using System.IO;
using RazorEngine;
using RazorEngine.Templating;

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

            var downloadLink = AppConfigService.AppConfig.DownloadLink;
            var tuple = new Tuple<List<string>, string>(names, downloadLink);
            var template = File.ReadAllText("Templates/NotificationEmail.cshtml");
            var result = Engine.Razor.RunCompile(template, $"template{DateTime.Now.Ticks}", model: tuple);

            return result;
        }
    }
}