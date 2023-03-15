using System;
using System.Collections.Generic;

namespace Fanex.DHWL.Business.Models
{
    public class ConfigSettingModel
    {
        public bool IsCheckEnoughData { get; set; }

        public string SentryClientKey { get; set; }

        public string LetstalkServiceApi { get; set; }

        public string CategorySentryLog { get; set; }

        public string Path { get; set; }

        public string TargetName { get; set; }

        public string LetstalkGroup { get; set; }

        public string Password { get; set; }

        public ReportType ReportType { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public Dictionary<string, ImageProperty> ImageSettings { get; set; } = new Dictionary<string, ImageProperty>();

        public Dictionary<int, ChartAreasPosition> AreasPosition { get; set; } = new Dictionary<int, ChartAreasPosition>();

        public Dictionary<string, LetstalkGroup> LetstalkGroups { get; set; } = new Dictionary<string, LetstalkGroup>();

        public bool IsAttachAdjument { get; set; }

        public int DataMode { get; set; } = 1;

        public decimal PerformanceThreshold { get; set; } = 200000;
    }
}