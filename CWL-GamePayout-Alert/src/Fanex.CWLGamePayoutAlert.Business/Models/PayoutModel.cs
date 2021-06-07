namespace Fanex.CWLGamePayoutAlert.Business.Models
{
    public class PayoutModel
    {
        public string Product { get; set; }

        public string GameType { get; set; }

        public int BetCount { get; set; }

        public decimal TotalBetsRM { get; set; }

        public decimal TotalWinsRM { get; set; }

        public decimal GameIncome
        {
            get
            {
                return TotalBetsRM - TotalWinsRM;
            }
        }

        public decimal GamePayout { get; set; }
    }
}