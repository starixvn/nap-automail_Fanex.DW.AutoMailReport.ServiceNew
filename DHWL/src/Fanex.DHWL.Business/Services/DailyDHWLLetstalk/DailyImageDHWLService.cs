using Fanex.DHWL.Business.Services.LicenseePerformance;
using System.Data;

namespace Fanex.DHWL.Business.Services.DailyDHWLLetstalk
{
    public interface DailyImageDHWLService
    {
        string Export(DataTable dailyRawData);
    }

    public class DailyImageDHWLServiceImp : DailyImageDHWLService
    {
        private readonly DailyExcelDHWLService excelDHWLService;
        private readonly ImageService imageService;
        private readonly MergeProductDataService mergeProductDataService;

        public DailyImageDHWLServiceImp(
            DailyExcelDHWLService excelDHWLService,
            ImageService imageService,
            MergeProductDataService mergeProductDataService)
        {
            this.excelDHWLService = excelDHWLService;
            this.imageService = imageService;
            this.mergeProductDataService = mergeProductDataService;
        }

        public string Export(DataTable dailyRawData)
        {
            mergeProductDataService.MergeVGamingIntoSabaC(dailyRawData);

            var excelPath = excelDHWLService.Export(dailyRawData);
            var lastRow = excelDHWLService.LastRowIndex;
            var image = AppConfigService.Config.ImageSettings["DHWL"];
            image.LastRow = lastRow;
            image.CropHeight = lastRow * image.HeightPerRow + image.AdjustHeight;
            var imagePath = imageService.ExportImage(excelPath, image.SheetName, image);

            return imagePath;
        }
    }
}