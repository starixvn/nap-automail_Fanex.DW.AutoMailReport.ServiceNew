using System.Data;

namespace Fanex.DHWL.Business.Services.LicenseePerformance
{
    public interface LicenseePerformanceImageService
    {
        string Export(DataTable licPerfRawData, string imageName);
    }

    public class LicenseePerformanceImageServiceImp : LicenseePerformanceImageService
    {
        private readonly LicenseePerformanceExcelService excelService;
        private readonly ImageService imageService;

        public LicenseePerformanceImageServiceImp(
            LicenseePerformanceExcelService excelService,
            ImageService imageService)
        {
            this.excelService = excelService;
            this.imageService = imageService;
        }

        public string Export(DataTable licPerfRawData, string imageName)
        {
            var excelPath = excelService.Export(licPerfRawData, imageName);
            var lastRow = excelService.LastRowIndex;
            var image = AppConfigService.Config.ImageSettings["LicenseePerformance"];
            image.LastRow = lastRow;
            image.CropHeight = lastRow * image.HeightPerRow + image.AdjustHeight;
            image.ImageName = imageName;
            var imagePath = imageService.ExportImage(excelPath, image.SheetName, image);

            return imagePath;
        }
    }
}