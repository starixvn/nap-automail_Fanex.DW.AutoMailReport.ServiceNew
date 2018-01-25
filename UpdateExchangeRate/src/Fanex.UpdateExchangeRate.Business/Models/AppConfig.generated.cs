using System;

namespace Fanex.UpdateExchangeRate.Business.Models
{
    public class AppConfig
    {
        public string ExportDataDirectory { get; set; }

        public string ExcelPasswordProtected { get; set; }

        public string Subject { get; set; }

        public string Title { get; set; }

        public Uri NapReportUrl { get; set; }
    }
}