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
        private readonly BaseDataTable baseDataTable;

        public string FileName { get; set; }

        public MonthlyImageDHWLServiceImp(
            MonthlyExcelDHWLService excelDhwlService,
            ImageService imageService,
            MergeProductDataService mergeProductDataService,
            BaseDataTable baseDataTable)
        {
            this.excelDhwlService = excelDhwlService;
            this.imageService = imageService;
            this.mergeProductDataService = mergeProductDataService;
            this.baseDataTable = baseDataTable;
        }

        public List<string> Export(DataTable monthlyRawData, DataTable dataReconciliation, DataTable dataBa,
            int reportId)
        {
            mergeProductDataService.MergeProductsIntoSabaGame(monthlyRawData);

            var monthlyRawDataScaled = baseDataTable.ThousandScale(monthlyRawData, 2);
            var dataReconciliationScaled = baseDataTable.ThousandScale(dataReconciliation, 2);
            var dataBaScaled = baseDataTable.ThousandScale(dataBa, 2);
            var excelPathScaled =
                excelDhwlService.Export(monthlyRawDataScaled, dataReconciliationScaled, dataBaScaled, reportId, true);
            var excelPath = excelDhwlService.Export(monthlyRawData, dataReconciliation, dataBa, reportId, false);
            FileName = excelPath;

            var imageDhwlMonthly = ExportDhwlMonthly(excelPathScaled);
            var imageAdjustmentMonthly = ExportAdjustmentMonthly(excelPathScaled);

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