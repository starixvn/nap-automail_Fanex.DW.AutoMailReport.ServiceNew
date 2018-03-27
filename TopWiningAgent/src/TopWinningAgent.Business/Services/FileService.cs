using System;
using System.IO;
using Fanex.Service.Core;
using RazorEngine;
using RazorEngine.Templating;

namespace TopWinningAgent.Business.Services
{
    public interface FileService
    {
        string ReadContent();

        string ReadContentWhenDataIsNotReady();
    }

    public class FileServiceImp : FileService
    {
        private readonly DateRangeService dateRangeService;
        private readonly RepositoryService repositoryService;

        public FileServiceImp(DateRangeService dateRangeService, RepositoryService repositoryService)
        {
            this.dateRangeService = dateRangeService;
            this.repositoryService = repositoryService;
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

        public string ReadContentWhenDataIsNotReady()
        {
            var templatePath = @"Templates\ContentWhenDataIsNotReady.cshtml";
            var emailTemplate = File.ReadAllText(templatePath);
            var lastDate = repositoryService.QueryLastDate();
            return Engine.Razor.RunCompile(emailTemplate, templatePath, lastDate.GetType(), lastDate);
        }
    }
}