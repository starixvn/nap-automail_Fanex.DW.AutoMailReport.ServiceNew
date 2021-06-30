using Fanex.DHWL.Business.Models;
using System.Collections.Generic;
using System.Data;
using System.Linq;

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
            var letstalkGroup = AppConfigService.Config.LetstalkGroups["LicenseePerformance"];
            var sbRawData = licPerfRawData.AsEnumerable()
                .Where(s => s[ColumnName.ProductGroup].Equals(ProductGroup.Sportsbook))
                .CopyToDataTable();

            var osRawData = licPerfRawData.AsEnumerable()
                .Where(s => !s[ColumnName.ProductGroup].Equals(ProductGroup.Sportsbook))
                .CopyToDataTable();

            var imagePath = imageService.Export(sbRawData, "SB-LicPef");
            var imagePath3rd = imageService.Export(osRawData, "3rd-LicPef");

            fileService.CopyFiles(letstalkGroup, new List<string> { imagePath, imagePath3rd });

            return messageService.SendMessage(new List<string> { imagePath, imagePath3rd }, AppConfigService.Config.LetstalkGroupLicPerf);
        }
    }
}