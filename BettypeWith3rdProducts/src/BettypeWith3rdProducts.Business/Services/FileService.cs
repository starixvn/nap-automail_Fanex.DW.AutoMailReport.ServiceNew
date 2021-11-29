using RazorEngine;
using RazorEngine.Templating;
using System;
using System.IO;

namespace BettypeWith3rdProducts.Business.Services
{
    public interface FileService
    {
        string ReadContentWhenDataIsAvailable(string excelFileName, string templateFile);
    }

    public class FileServiceImp : FileService
    {
        private readonly ApplicationConfigurationService appConfig;

        public FileServiceImp(ApplicationConfigurationService appConfig)
        {
            this.appConfig = appConfig;
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