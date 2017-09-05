using System;
using System.IO;
using Fanex.Service.Core;
using RazorLight;

namespace EventMarketOffer.Business.Services
{
    public interface FileService
    {
        string ReadContentWhenDataIsNotReady();

        string ReadContentWhenDataIsAvailable(string excelFileName);
    }

    public class FileServiceImp : FileService
    {
        private readonly RepositoryService repositoryService;
        private readonly AppConfigurationService appConfig;

        public FileServiceImp(RepositoryService repositoryService, AppConfigurationService appConfig)
        {
            this.repositoryService = repositoryService;
            this.appConfig = appConfig;
        }

        public string ReadContentWhenDataIsAvailable(string excelFileName)
        {
            var result = string.Empty;
            var root = $@"{Directory.GetCurrentDirectory()}\Templates";
            var engine = EngineFactory.CreatePhysical(root);
            var tuple = new Tuple<string, string>(excelFileName, appConfig.Configurations.NapReportUrl.AbsoluteUri);

            result = engine.Parse("ContentWithAttachment.cshtml", tuple);

            return result;
        }

        public string ReadContentWhenDataIsNotReady()
        {
            var result = string.Empty;
            var root = $@"{Directory.GetCurrentDirectory()}\Templates";
            var engine = EngineFactory.CreatePhysical(root);
            var lastDate = repositoryService.QueryLastDate();
            result = engine.Parse("ContentWhenDataIsNotReady.cshtml", lastDate);

            return result;
        }
    }
}