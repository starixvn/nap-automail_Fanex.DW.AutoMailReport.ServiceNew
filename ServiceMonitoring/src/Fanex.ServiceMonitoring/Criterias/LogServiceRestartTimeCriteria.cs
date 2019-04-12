using Fanex.Data.Repository;

namespace Fanex.ServiceMonitoring.Criterias
{
    public class LogServiceRestartTimeCriteria : NonQueryCommand
    {
        public int ServiceId { get; set; }

        public override string GetSettingKey() => "ServiceMonitoringDetail_Insert";

        public override bool IsValid() => true;
    }
}