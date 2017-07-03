using System;

namespace NegativeBalance.Business.Models
{
    public class MatchSettleLog
    {
        public int LogId { get; set; }

        public DateTime SettlementTime { get; set; }

        public string Action { get; set; } = string.Empty;

        public string ActionCS { get; set; } = string.Empty;

        public int FinalHomeScore { get; set; }

        public int FinalAwayScore { get; set; }

        public int HTHomeScore { get; set; }

        public int HTAwayScore { get; set; }

        public string Result { get; set; } = string.Empty;

        public string Remark { get; set; } = string.Empty;
    }
}