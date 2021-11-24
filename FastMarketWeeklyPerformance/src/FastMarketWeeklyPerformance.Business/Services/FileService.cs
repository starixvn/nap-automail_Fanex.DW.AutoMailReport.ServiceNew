using RazorEngine;
using RazorEngine.Templating;
using System.IO;

namespace FastMarketWeeklyPerformance.Business.Services
{
    public interface FileService
    {
        string ReadContentWhenDataIsAvailable();
    }

    public class FileServiceImp : FileService
    {
        private readonly ApplicationConfigurationService appConfigService;
        private readonly DateRangeService dateRangeService;

        public FileServiceImp(ApplicationConfigurationService appConfig, DateRangeService dateRangeService)
        {
            this.appConfigService = appConfig;
            this.dateRangeService = dateRangeService;
        }

        public string ReadContentWhenDataIsAvailable()
        {
            var dateRange = dateRangeService.DateRange(appConfigService.Configurations.ReportDateRange);
            var templatePath = $@"Templates\ContentWithAttachment.cshtml";
            var emailTemplate = File.ReadAllText(templatePath);
            var todate = $"{dateRange.ToDate:MMMM dd, yyyy}";
            return Engine.Razor.RunCompile(emailTemplate, templatePath, todate.GetType(), todate);
        }
    }
}