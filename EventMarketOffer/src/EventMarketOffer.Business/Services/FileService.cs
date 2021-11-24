using RazorLight;
using System;
using System.IO;

namespace EventMarketOffer.Business.Services
{
    public interface FileService
    {
        string ReadContentWhenDataIsAvailable(string excelFileName);
    }

    public class FileServiceImp : FileService
    {
        private readonly AppConfigurationService appConfig;

        public FileServiceImp(AppConfigurationService appConfig)
        {
            this.appConfig = appConfig;
        }

        public string ReadContentWhenDataIsAvailable(string excelFileName)
        {
            var root = $@"{Directory.GetCurrentDirectory()}\Templates";
            var engine = EngineFactory.CreatePhysical(root);
            var tuple = new Tuple<string, string>(excelFileName, appConfig.Configurations.NapReportUrl.AbsoluteUri);

            var result = engine.Parse("ContentWithAttachment.cshtml", tuple);

            return result;
        }
    }
}