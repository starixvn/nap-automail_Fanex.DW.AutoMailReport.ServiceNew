using Fanex.DHWL.Business.Models;
using Fanex.DHWL.Business.Services.LicenseePerformance;
using System.Collections.Generic;
using System.Data;

namespace Fanex.DHWL.Business.Services.Margin
{
    public interface ImageMarginService
    {
        string Export(DataTable dailyRawData, bool isDaily, int reportId);

        List<MarginModel> GrandTotalList { get; set; }
    }

    public class ImageMarginServiceImp : ImageMarginService
    {
        private readonly ExcelMarginService excelMarginService;
        private readonly ImageService imageService;
        private readonly MergeProductDataService mergeProductDataService;

        public ImageMarginServiceImp(
            ExcelMarginService excelMarginService,
            ImageService imageService,
            MergeProductDataService mergeProductDataService)
        {
            this.excelMarginService = excelMarginService;
            this.imageService = imageService;
            this.mergeProductDataService = mergeProductDataService;
        }

        public List<MarginModel> GrandTotalList { get; set; }

        public string Export(DataTable dailyRawData, bool isDaily, int reportId)
        {
            mergeProductDataService.MergeVGamingIntoSabaC(dailyRawData);

            var excelPath = excelMarginService.Export(dailyRawData, isDaily, reportId);
            var lastRow = excelMarginService.LastRowIndex;
            GrandTotalList = excelMarginService.GrandTotalList;

            var image = AppConfigService.Config.ImageSettings["Margin"];
            image.LastRow = lastRow;
            image.CropHeight = lastRow * image.HeightPerRow + image.AdjustHeight;
            image.ImageName = isDaily ? "DHWL-Margin" : "DHWL-Margin-MTD";
            var imagePath = imageService.ExportImage(excelPath, image.SheetName, image);

            return imagePath;
        }
    }
}