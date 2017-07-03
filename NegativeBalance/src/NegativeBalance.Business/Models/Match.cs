using System.Collections.Generic;

namespace NegativeBalance.Business.Models
{
    public class Match
    {
        public MatchInfo MatchInfo { get; set; } = new MatchInfo();

        public List<MatchSettleLog> MatchSettleLogList { get; set; } = new List<MatchSettleLog>();

        public List<CustomerBalanceInfo> CustomerInfoList { get; set; } = new List<CustomerBalanceInfo>();

        public string LicenseeName { get; set; } = string.Empty;
    }
}