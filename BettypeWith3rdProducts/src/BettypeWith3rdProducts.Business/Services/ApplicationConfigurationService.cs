using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using BettypeWith3rdProducts.Business.Models;
using Fanex.Common;
using Newtonsoft.Json;

namespace BettypeWith3rdProducts.Business.Services
{
    public interface ApplicationConfigurationService
    {
        string ExportDataDirectory { get; }

        string ExcelPasswordProtected { get; }

        DateRange ReportType { get; }

        bool IsCheckEnoughData { get; }

        string NapReportURL { get; }

        int FunctionIdNodata { get; }

        int FunctionIdWithoutAttachment { get; }

        string Subject { get; }

        string Title { get; }

        List<SourceMapping> SourceMappings { get; }
    }

    public class ApplicationConfigurationServiceImp : ApplicationConfigurationService
    {
        public string ExportDataDirectory => ConfigurationManager.AppSettings["ExportDataDirectory"];

        public string ExcelPasswordProtected => ConfigurationManager.AppSettings["ExcelPasswordProtected"];

        public DateRange ReportType
        {
            get
            {
                var reportType = ConfigurationManager.AppSettings["ReportType"];
                DateRange dateRange;
                Enum.TryParse(reportType, true, out dateRange);
                return dateRange;
            }
        }

        public bool IsCheckEnoughData => ConfigurationManager.AppSettings["IsCheckEnoughData"] == "true";

        public string NapReportURL => ConfigurationManager.AppSettings["NapReportURL"];

        public int FunctionIdNodata => Converter.To<int>(ConfigurationManager.AppSettings["FunctionIdNodata"]);

        public int FunctionIdWithoutAttachment => Converter.To<int>(ConfigurationManager.AppSettings["FunctionIdWithoutAttachment"]);

        public string Subject => ConfigurationManager.AppSettings["Subject"];

        public string Title => ConfigurationManager.AppSettings["Title"];

        public List<SourceMapping> SourceMappings
        {
            get
            {
                var text = File.ReadAllText(@"Templates\SourceMapping.json");
                return JsonConvert.DeserializeObject<List<SourceMapping>>(text);
            }
        }
    }
}