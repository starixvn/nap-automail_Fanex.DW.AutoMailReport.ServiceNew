using System.Collections.Generic;
using System.Data;

namespace Fanex.DHWL.Business.Services.LicenseePerformance
{
    public interface LicenseePerformanceService
    {
        Dictionary<string, bool> Execute(DataTable licPerfRawData);
    }

    public class LicenseePerformanceServiceImp : LicenseePerformanceService
    {
        private readonly LicenseePerformanceImageService imageService;
        private readonly MessageService messageService;
        private readonly FileService fileService;

        public LicenseePerformanceServiceImp(
            LicenseePerformanceImageService imageService,
            MessageService messageService,
            FileService fileService)
        {
            this.imageService = imageService;
            this.messageService = messageService;
            this.fileService = fileService;
        }

        public Dictionary<string, bool> Execute(DataTable licPerfRawData)
        {
            var imagePath = imageService.Export(licPerfRawData);
            var letstalkGroup = AppConfigService.Config.LetstalkGroups["LicenseePerformance"];
            fileService.CopyFiles(letstalkGroup, new List<string> { imagePath });

            return messageService.SendMessage(new List<string> { imagePath }, AppConfigService.Config.LetstalkGroup);
        }
    }
}