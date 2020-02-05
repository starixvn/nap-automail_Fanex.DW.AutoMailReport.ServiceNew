namespace Fanex.N88DailyWL.Business.Models
{
    public class N88DailyWLModel
    {
        public string Date { get; set; }

        public int ActiveMember { get; set; }

        public int DepositMember { get; set; }

        public int BetCount { get; set; }

        public decimal MemberTO { get; set; }

        public decimal MemberWL { get; set; }

        public decimal CompanyTO { get; set; }

        public decimal CompanyTO_Live { get; set; }

        public decimal CompanyTO_NonLive { get; set; }

        public decimal CompanyTO_Mix { get; set; }

        public decimal CompanyIncome { get; set; }

        public decimal CompanyIncome_Live { get; set; }

        public decimal CompanyIncome_NonLive { get; set; }

        public decimal CompanyIncome_Mix { get; set; }

        public decimal CompanyMargin { get; set; }
    }
}