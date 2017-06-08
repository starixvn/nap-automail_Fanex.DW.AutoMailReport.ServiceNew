using System;
using System.IO;
using RazorEngine;
using RazorEngine.Templating;

namespace BettypeWith3rdProducts.Business.Services
{
    public interface FileService
    {
        string ReadContentWhenDataIsNotReady();

        string ReadContentWhenDataIsAvailable(string excelFileName, string templateFile);
    }

    public class FileServiceImp : FileService
    {
        private readonly RepositoryService repositoryService;
        private readonly ApplicationConfigurationService appConfig;

        public FileServiceImp(RepositoryService repositoryService, ApplicationConfigurationService appConfig)
        {
            this.repositoryService = repositoryService;
            this.appConfig = appConfig;
        }

        public string ReadContentWhenDataIsNotReady()
        {
            var templatePath = @"Templates\ContentWhenDataIsNotReady.cshtml";
            var emailTemplate = File.ReadAllText(templatePath);
            var lastDate = repositoryService.QueryLastDate();
            return Engine.Razor.RunCompile(emailTemplate, templatePath, lastDate.GetType(), lastDate);
        }

        public string ReadContentWhenDataIsAvailable(string excelFileName, string templateFile)
        {
            var templatePath = $@"Templates\{templateFile}";
            var emailTemplate = File.ReadAllText(templatePath);
            var tuple = new Tuple<string, string>(excelFileName, appConfig.NapReportUrl.AbsoluteUri);
            return Engine.Razor.RunCompile(emailTemplate, templatePath, tuple.GetType(), tuple);
        }
    }
}