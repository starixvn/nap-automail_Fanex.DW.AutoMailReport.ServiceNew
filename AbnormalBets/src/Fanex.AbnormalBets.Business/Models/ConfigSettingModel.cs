using System;
using System.Collections.Generic;

namespace Fanex.AbnormalBets.Business.Models
{
    public class ConfigSettingModel
    {
        public string SentryClientKey { get; set; }

        public string CategorySentryLog { get; set; }

        public bool IsManual { get; set; }

        public bool IsTest { get; set; }

        public bool IsIgnoredCheckMailSent { get; set; }

        public DateTime RptDate { get; set; }

        public string Path { get; set; }

        public string Password { get; set; }

        public List<SiteModel> Sites { get; set; } 
    }
}