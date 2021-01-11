using System.Data;

namespace Fanex.MarginChart.Business.Services.DailyDHWLLetstalk
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
            return string.Empty;
        }
    }
}