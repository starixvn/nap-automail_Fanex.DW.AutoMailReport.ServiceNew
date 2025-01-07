using Fanex.DHWL.Business.Services.LicenseePerformance;
using System.Collections.Generic;
using System.Data;

namespace Fanex.DHWL.Business.Services.MonthlyDHWLLetstalk
{
    public interface MonthlyImageDHWLService
    {
        List<string> Export(DataTable monthlyRawData, DataTable dataReconciliation, DataTable dataBa, int reportId);

        string FileName { get; set; }
    }

    public class MonthlyImageDHWLServiceImp : MonthlyImageDHWLService
    {
        private readonly MonthlyExcelDHWLService excelDhwlService;
        private readonly ImageService imageService;
        private readonly MergeProductDataService mergeProductDataService;

        public string FileName
        {
            get; set;
        }

        public MonthlyImageDHWLServiceImp(
            MonthlyExcelDHWLService excelDhwlService,
            ImageService imageService,
            MergeProductDataService mergeProductDataService)
        {
            this.excelDhwlService = excelDhwlService;
            this.imageService = imageService;
            this.mergeProductDataService = mergeProductDataService;
        }

        public List<string> Export(DataTable monthlyRawData, DataTable dataReconciliation, DataTable dataBa, int reportId)
        {
            mergeProductDataService.MergeProductsIntoSabaGame(monthlyRawData);
            var excelPath = excelDhwlService.Export(monthlyRawData, dataReconciliation, dataBa, reportId);
            FileName = excelPath;

            var imageDhwlMonthly = ExportDhwlMonthly(excelPath);
            var imageAdjustmentMonthly = ExportAdjustmentMonthly(excelPath);

            var result = AppConfigService.Config.IsAttachAdjument
                ? new List<string> { imageDhwlMonthly, imageAdjustmentMonthly }
                : new List<string> { imageDhwlMonthly, };

            return result;
        }

        private string ExportDhwlMonthly(string excelPath)
        {
            var lastRow = excelDhwlService.LastRowIndex;
            var image = AppConfigService.Config.ImageSettings["DHWL"];
            image.LastRow = lastRow;
            image.CropHeight = lastRow * image.HeightPerRow + image.AdjustHeight;
            return imageService.ExportImage(excelPath, image.SheetName, image);
        }

        private string ExportAdjustmentMonthly(string excelPath)
        {
            var lastRow = excelDhwlService.LastAdjustmentRowIndex;
            var image = AppConfigService.Config.ImageSettings["Adjustment"];
            image.LastRow = lastRow;
            image.CropHeight = lastRow * image.HeightPerRow + image.AdjustHeight;
            return imageService.ExportImage(excelPath, image.SheetName, image);
        }
    }
}