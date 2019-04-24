using System.IO;
using Fanex.Service.Core;
using RazorEngine;
using RazorEngine.Templating;

namespace FastMarketWeeklyPerformance.Business.Services
{
    public interface FileService
    {
        string ReadContentWhenDataIsNotReady();

        string ReadContentWhenDataIsAvailable();
    }

    public class FileServiceImp : FileService
    {
        private readonly RepositoryService repositoryService;
        private readonly ApplicationConfigurationService appConfigService;
        private readonly DateRangeService dateRangeService;

        public FileServiceImp(RepositoryService repositoryService, ApplicationConfigurationService appConfig, DateRangeService dateRangeService)
        {
            this.repositoryService = repositoryService;
            this.appConfigService = appConfig;
            this.dateRangeService = dateRangeService;
        }

        public string ReadContentWhenDataIsNotReady()
        {
            const string TemplatePath = @"Templates\ContentWhenDataIsNotReady.cshtml";
            var emailTemplate = File.ReadAllText(TemplatePath);
            var lastDate = repositoryService.QueryLastDate();
            return Engine.Razor.RunCompile(emailTemplate, TemplatePath, lastDate.GetType(), lastDate);
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