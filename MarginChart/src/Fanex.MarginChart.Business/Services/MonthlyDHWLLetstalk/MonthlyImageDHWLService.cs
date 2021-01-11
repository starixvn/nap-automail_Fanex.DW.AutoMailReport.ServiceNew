using System.Data;

namespace Fanex.MarginChart.Business.Services.MonthlyDHWLLetstalk
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
            return string.Empty;
        }
    }
}