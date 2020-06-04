namespace Fanex.ESportModelingData.Business.Models
{
    public class RawModelingDataModel
    {
        public int Level { get; set; }

        public string Sport { get; set; }

        public string Game { get; set; }

        public string BetTypeGroup { get; set; }

        public string BetType { get; set; }

        public string Status { get; set; }

        public string Customer { get; set; }

        public int BetCount { get; set; }

        public decimal MemberTurnover { get; set; }

        public decimal MemberWinLoss { get; set; }

        public decimal VirtualrateSetting { get; set; }
    }
}