using RazorEngine;
using RazorEngine.Templating;
using System;
using System.IO;

namespace BetradarVirtualSportsMonthly.Business.Services
{
    public interface FileService
    {
        string ReadContentWhenDataIsAvailable();
    }

    public class FileServiceImp : FileService
    {
        private readonly DateService dateService;

        public FileServiceImp(DateService dateService)
        {
            this.dateService = dateService;
        }

        public string ReadContentWhenDataIsAvailable()
        {
            var templatePath = $@"Templates\ContentWithAttachment.cshtml";
            var emailTemplate = File.ReadAllText(templatePath);
            var dates = new Tuple<DateTime, DateTime>(dateService.FromDate, dateService.ToDate);
            return Engine.Razor.RunCompile(emailTemplate, templatePath, dates.GetType(), dates);
        }
    }
}