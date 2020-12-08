using System.Data;

namespace Fanex.DHWL.Business.Services.Margin
{
    public interface ImageMarginService
    {
        string Export(DataTable dailyRawData, bool isDaily);
    }

    public class ImageMarginServiceImp : ImageMarginService
    {
        private readonly ExcelMarginService excelMarginService;
        private readonly ImageService imageService;

        public ImageMarginServiceImp(
            ExcelMarginService excelMarginService,
            ImageService imageService)
        {
            this.excelMarginService = excelMarginService;
            this.imageService = imageService;
        }

        public string Export(DataTable dailyRawData, bool isDaily)
        {
            var excelPath = excelMarginService.Export(dailyRawData, isDaily);
            var lastRow = excelMarginService.LastRowIndex;
            var image = AppConfigService.Config.ImageSettings["Margin"];
            image.LastRow = lastRow;
            image.CropHeight = lastRow * image.HeightPerRow + image.AdjustHeight;
            image.ImageName = isDaily ? "DHWL-Margin" : "DHWL-Margin-MTD";
            var imagePath = imageService.ExportImage(excelPath, image.SheetName, image);

            return imagePath;
        }
    }
}