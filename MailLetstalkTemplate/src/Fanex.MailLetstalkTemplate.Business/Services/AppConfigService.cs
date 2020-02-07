using Fanex.MailLetstalkTemplate.Business.Models;
using System.Collections.Generic;

namespace Fanex.MailLetstalkTemplate.Business.Services
{
    public static class AppConfigService
    {
        public static BaseSetting BaseSetting { get; set; }

        public static List<ReportModel> Reports { get; set; }

        public static ConfigSettingModel Config { get; set; }
    }
}