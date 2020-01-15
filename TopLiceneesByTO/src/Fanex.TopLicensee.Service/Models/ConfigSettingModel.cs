using System;
using System.Collections.Generic;

namespace Fanex.TopLicensee.Service.Models
{
    public class ConfigSettingModel
    {
        public string SentryClientKey { get; set; }

        public string CategorySentryLog { get; set; }

        public string TargetName { get; set; }

        public string Group { get; set; }

        public bool IsCheckEnoughData { get; set; }

        public string Path { get; set; }

        public string Password { get; set; }

        public int FunctionIdNodata { get; set; }

        public bool IsManual { get; set; }

        public DateTime RptDate { get; set; }

        public List<SectionModel> Sports { get; set; } = new List<SectionModel>();

        public ImageProperty Image { get; set; } = new ImageProperty();
    }
}