using System;
using System.IO;
using Fanex.UpdateExchangeRate.Business.Models;
using RazorEngine;
using RazorEngine.Templating;

namespace Fanex.UpdateExchangeRate.Business.Services
{
    public interface FileService
    {
        string ReadContent(string excelFileName, int numberOfUpdates);
    }

    public class FileServiceImp : FileService
    {
        private readonly AppConfig appConfig;

        public FileServiceImp(ApplicationConfigurationService appConfigService)
        {
            appConfig = appConfigService.Configurations;
        }

        public string ReadContent(string excelFileName, int numberOfUpdates)
        {
            var templatePath = $@"Templates\ContentWithAttachment.cshtml";
            var emailTemplate = File.ReadAllText(templatePath);
            var tuple = new Tuple<string, string, int>(excelFileName, appConfig.NapReportUrl.AbsoluteUri, numberOfUpdates);
            return Engine.Razor.RunCompile(emailTemplate, templatePath, tuple.GetType(), tuple);            
        }
    }
}