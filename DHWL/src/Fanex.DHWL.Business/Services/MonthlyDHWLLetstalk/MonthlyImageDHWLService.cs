using System.Data;

namespace Fanex.DHWL.Business.Services.MonthlyDHWLLetstalk
{
    public interface MonthlyImageDHWLService
    {
        string Export(DataTable monthlyRawData);
    }

    public class MonthlyImageDHWLServiceImp : MonthlyImageDHWLService
    {
        private readonly MonthlyExcelDHWLService excelDHWLService;
        private readonly ImageService imageService;

        public MonthlyImageDHWLServiceImp(
            MonthlyExcelDHWLService excelDHWLService,
            ImageService imageService)
        {
            this.excelDHWLService = excelDHWLService;
            this.imageService = imageService;
        }

        public string Export(DataTable monthlyRawData)
        {
            var excelPath = excelDHWLService.Export(monthlyRawData);
            var lastRow = excelDHWLService.LastRowIndex;
            var image = AppConfigService.Config.ImageSettings["DHWL"];
            image.LastRow = lastRow;
            image.CropHeight = lastRow * image.HeightPerRow + image.AdjustHeight;
            var imagePath = imageService.ExportImage(excelPath, image.SheetName, image);

            return imagePath;
        }
    }
}