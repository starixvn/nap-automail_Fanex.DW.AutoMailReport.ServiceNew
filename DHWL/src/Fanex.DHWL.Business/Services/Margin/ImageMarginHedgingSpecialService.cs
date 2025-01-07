using Fanex.DHWL.Business.Models;
using Fanex.DHWL.Business.Services.LicenseePerformance;
using System.Collections.Generic;
using System.Data;

namespace Fanex.DHWL.Business.Services.Margin
{
    public interface ImageMarginHedgingSpecialService
    {
        string Export(DataTable dailyRawData, bool isDaily, int reportId);

        List<MarginHedgingSpecialModel> GrandTotalList { get; set; }
    }

    public class ImageMarginHedgingSpecialServiceImp : ImageMarginHedgingSpecialService
    {
        private readonly ExcelMarginHedgingSpecialService excelMarginService;
        private readonly ImageService imageService;
        private readonly MergeProductDataService mergeProductDataService;

        public ImageMarginHedgingSpecialServiceImp(
            ExcelMarginHedgingSpecialService excelMarginService,
            ImageService imageService,
            MergeProductDataService mergeProductDataService)
        {
            this.excelMarginService = excelMarginService;
            this.imageService = imageService;
            this.mergeProductDataService = mergeProductDataService;
        }

        public List<MarginHedgingSpecialModel> GrandTotalList { get; set; }

        public string Export(DataTable dailyRawData, bool isDaily, int reportId)
        {
            mergeProductDataService.MergeProductsIntoSabaGame(dailyRawData);

            var excelPath = excelMarginService.Export(dailyRawData, isDaily, reportId);
            var lastRow = excelMarginService.LastRowIndex;
            GrandTotalList = excelMarginService.GrandTotalList;

            var image = AppConfigService.Config.ImageSettings["MarginHedgingSpecial"];
            image.LastRow = lastRow;
            image.CropHeight = lastRow * image.HeightPerRow + image.AdjustHeight;
            image.ImageName = isDaily ? "DHWL-Margin-HedgingSpecial" : "DHWL-Margin-HedgingSpecial-MTD";
            var imagePath = imageService.ExportImage(excelPath, image.SheetName, image);

            return imagePath;
        }
    }
}