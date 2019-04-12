using Fanex.Data.Repository;

namespace Fanex.ServiceMonitoring.Criterias
{
    public class GetAllScheduleCriteria : CriteriaBase
    {
        public string MailName { get; set; }

        public int SiteId { get; set; }

        public override bool IsValid() => true;

        public override string GetSettingKey()
        {
            return "Mail_Report_GetAllSchedule";
        }
    }
}