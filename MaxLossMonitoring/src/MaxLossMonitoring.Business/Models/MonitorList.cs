using System.Collections.Generic;

namespace MaxLossMonitoring.Business.Models
{
    public class MonitorList
    {
        public MonitorList()
        {
            LevelNameForEmail = string.Empty;
            LevelNameForLetstalk = string.Empty;
            Rows = new List<MonitorListRaw>();
        }

        public int LevelId { get; set; }

        public string LevelNameForEmail { get; set; }

        public string LevelNameForLetstalk { get; set; }

        public List<MonitorListRaw> Rows { get; set; }
    }
}