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

        public DailyImageDHWLServiceImp(
            DailyExcelDHWLService excelDHWLService,
            ImageService imageService)
        {
            this.excelDHWLService = excelDHWLService;
            this.imageService = imageService;
        }

        public string Export(DataTable dailyRawData)
        {
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