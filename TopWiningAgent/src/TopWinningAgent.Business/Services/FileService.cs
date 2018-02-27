using System;
using System.IO;
using RazorEngine;
using RazorEngine.Templating;

namespace TopWinningAgent.Business.Services
{
    public interface FileService
    {
        string ReadContent();
    }

    public class FileServiceImp : FileService
    {
        private readonly DateRangeService dateRangeService;

        public FileServiceImp(DateRangeService dateRangeService)
        {
            this.dateRangeService = dateRangeService;
        }

        public string ReadContent()
        {
            var templatePath = $@"Templates\ContentWithAttachment.cshtml";
            var emailTemplate = File.ReadAllText(templatePath);
            var fromdate = $"{dateRangeService.FromDate(1):MM/dd/yyyy}";
            var todate = $"{dateRangeService.ToDate():MM/dd/yyyy}";
            var tuple = new Tuple<string, string>(fromdate, todate);
            return Engine.Razor.RunCompile(emailTemplate, templatePath, tuple.GetType(), tuple);
        }
    }
}