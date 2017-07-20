using System.IO;
using Fanex.Service.Core;
using RazorEngine;
using RazorEngine.Templating;

namespace BetradarVirtualSports.Business.Services
{
    public interface FileService
    {
        string ReadContentWhenDataIsNotReady();

        string ReadContentWhenDataIsAvailable();
    }

    public class FileServiceImp : FileService
    {
        private readonly DateService dateService;
        private readonly RepositoryService repositoryService;

        public FileServiceImp(DateService dateService, RepositoryService repositoryService)
        {
            this.dateService = dateService;
            this.repositoryService = repositoryService;
        }

        public string ReadContentWhenDataIsAvailable()
        {
            var date = dateService.Date;
            var templatePath = $@"Templates\ContentWithAttachment.cshtml";
            var emailTemplate = File.ReadAllText(templatePath);
            var todate = $"{date:MM/dd/yyyy}";
            return Engine.Razor.RunCompile(emailTemplate, templatePath, todate.GetType(), todate);
        }

        public string ReadContentWhenDataIsNotReady()
        {
            var templatePath = $@"Templates\ContentWhenDataIsNotReady.cshtml";
            var emailTemplate = File.ReadAllText(templatePath);
            var date = repositoryService.QueryLastDate();
            return Engine.Razor.RunCompile(emailTemplate, templatePath, date.GetType(), date);
        }
    }
}