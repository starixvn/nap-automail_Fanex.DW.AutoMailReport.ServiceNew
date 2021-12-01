using RazorEngine;
using RazorEngine.Templating;
using System;
using System.IO;

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
            var fromdate = $"{dateRangeService.FromDate(1):MMM dd, yyyy}";
            var todate = $"{dateRangeService.ToDate():MMM dd, yyyy}";
            var tuple = new Tuple<string, string>(fromdate, todate);
            return Engine.Razor.RunCompile(emailTemplate, templatePath, tuple.GetType(), tuple);
        }
    }
}