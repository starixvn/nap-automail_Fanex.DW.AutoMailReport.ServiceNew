using System;
using System.Collections.Generic;

namespace Fanex.TopPunterCasino.Business.Models
{
    public class ConfigSettingModel
    {
        public bool IsCheckEnoughData { get; set; }

        public string SentryClientKey { get; set; }

        public string CategorySentryLog { get; set; }

        public string Path { get; set; }

        public string InternalPath { get; set; }

        public string TargetName { get; set; }

        public string Password { get; set; }

        public int FunctionIdNodata { get; set; }

        public bool IsManual { get; set; }

        public DateTime RptDate { get; set; }

        public string TimeRange { get; set; }

        public Dictionary<string, ImageProperty> ImageSettings { get; set; } = new Dictionary<string, ImageProperty>();

        public Dictionary<CasinoType, TopPunterCasinoSetting> CasinoTypes { get; set; }

        public int TotalTurnover { get; set; } = 50000;

        public string NonSportProductIds { get; set; } = "8,28,40,50,5,27,3";

        public bool IsSentDirectly { get; set; }

        public string OneTimePath { get; set; }
    }
}