namespace Fanex.MarginChart.Business.Services.Margin
{
    public interface ImageMarginService
    {
        string Export(string excelPath, bool isDaily, string category, int lastRowIndex);
    }

    public class ImageMarginServiceImp : ImageMarginService
    {
        private readonly ImageService imageService;

        public ImageMarginServiceImp(ImageService imageService)
        {
            this.imageService = imageService;
        }

        public string Export(string excelPath, bool isDaily, string category, int lastRowIndex)
        {
            var image = AppConfigService.Config.ImageSettings["Margin"];
            image.LastRow = lastRowIndex;
            image.CropHeight = lastRowIndex * image.HeightPerRow + image.AdjustHeight;
            image.ImageName = isDaily 
                ? $"Daily-{category}-Margin" 
                : $"MTD-{category}-Margin";
            var imagePath = imageService.ExportImage(excelPath, image.SheetName, category, image);

            return imagePath;
        }
    }
}