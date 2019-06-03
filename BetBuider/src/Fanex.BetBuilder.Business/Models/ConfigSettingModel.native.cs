using System;

namespace Fanex.BetBuilder.Business.Models
{
    public class ConfigSettingModel
    {
        public string SentryClientKey { get; set; }

        public string CategorySentryLog { get; set; }

        public bool IsManual { get; set; }

        public DateTime WinlostDate { get; set; }

        public string FilePath { get; set; }

        public string Password { get; set; }                
    }
}