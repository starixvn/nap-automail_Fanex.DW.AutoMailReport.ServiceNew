using System;

namespace Fanex.IBOPerformance.Service.Models
{
    public class ConfigSettingModel
    {
        public string SentryClientKey { get; set; }

        public string CategorySentryLog { get; set; }

        public bool IsCheckEnoughData { get; set; }

        public string Path { get; set; }

        public string Password { get; set; }

        public int CurrencyId { get; set; }

        public string Site { get; set; }

        public int FunctionIdNodata { get; set; }

        public bool IsManual { get; set; }

        public DateTime Yesterday { get; set; }
    }
}