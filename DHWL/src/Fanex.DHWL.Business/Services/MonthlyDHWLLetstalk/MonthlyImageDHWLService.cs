using Fanex.DHWL.Business.Services.LicenseePerformance;
using System.Collections.Generic;
using System.Data;

namespace Fanex.DHWL.Business.Services.MonthlyDHWLLetstalk
{
    public interface MonthlyImageDHWLService
    {
        List<string> Export(DataTable monthlyRawData, DataTable dataReconciliation);

        string FileName { get; set; }
    }

    public class MonthlyImageDHWLServiceImp : MonthlyImageDHWLService
    {
        private readonly MonthlyExcelDHWLService excelDHWLService;
        private readonly ImageService imageService;
        private readonly MergeProductDataService mergeProductDataService;

        public string FileName
        {
            get; set;
        }

        public MonthlyImageDHWLServiceImp(
            MonthlyExcelDHWLService excelDHWLService,
            ImageService imageService,
            MergeProductDataService mergeProductDataService)
        {
            this.excelDHWLService = excelDHWLService;
            this.imageService = imageService;
            this.mergeProductDataService = mergeProductDataService;
        }

        public List<string> Export(DataTable monthlyRawData, DataTable dataReconciliation)
        {
            mergeProductDataService.MergeVGamingIntoSabaC(monthlyRawData);
            var excelPath = excelDHWLService.Export(monthlyRawData, dataReconciliation);
            FileName = excelPath;
            var imageDHWLMonthly = ExportDHWLMonthly(excelPath);
            var imageAdjustmentMonthly = ExportAdjustmentMonthly(excelPath);

            var result = AppConfigService.Config.IsAttachAdjument
                ? new List<string> { imageDHWLMonthly, imageAdjustmentMonthly }
                : new List<string> { imageDHWLMonthly, };

            return result;
        }

        private string ExportDHWLMonthly(string excelPath)
        {
            var lastRow = excelDHWLService.LastRowIndex;
            var image = AppConfigService.Config.ImageSettings["DHWL"];
            image.LastRow = lastRow;
            image.CropHeight = lastRow * image.HeightPerRow + image.AdjustHeight;
            return imageService.ExportImage(excelPath, image.SheetName, image);
        }

        private string ExportAdjustmentMonthly(string excelPath)
        {
            var lastRow = excelDHWLService.LastAdjustmentRowIndex;
            var image = AppConfigService.Config.ImageSettings["Adjustment"];
            image.LastRow = lastRow;
            image.CropHeight = lastRow * image.HeightPerRow + image.AdjustHeight;
            return imageService.ExportImage(excelPath, image.SheetName, image);
        }
    }
}