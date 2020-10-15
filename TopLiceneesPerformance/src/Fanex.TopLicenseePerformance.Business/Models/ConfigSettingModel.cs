using System;
using System.Collections.Generic;

namespace Fanex.TopLicensee.Service.Models
{
    public class ConfigSettingModel
    {
        public string SentryClientKey { get; set; }

        public string CategorySentryLog { get; set; }

        public string TargetName { get; set; }

        public string LicenseeVirtualGroup { get; set; }

        public bool IsCheckEnoughData { get; set; }

        public string Path { get; set; }

        public string Password { get; set; }

        public int FunctionIdNodata { get; set; }

        public bool IsManual { get; set; }

        public DateTime RptDate { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public Dictionary<string, List<SectionModel>> Sports { get; set; } = new Dictionary<string, List<SectionModel>>();

        public Dictionary<string, ImageProperty> Image { get; set; } = new Dictionary<string, ImageProperty>();

        public bool IsSentDirectly { get; set; }

        public string InternalPath { get; set; }
    }
}