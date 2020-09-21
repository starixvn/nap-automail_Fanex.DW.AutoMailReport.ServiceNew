namespace Fanex.Dangerous789YCustomer.Business.Models
{
    public class PunterModel
    {
        public string UserName { get; set; }

        public string CurrencyName { get; set; }

        public decimal Turnover1 { get; set; }

        public decimal Winloss1 { get; set; }

        public decimal Margin1
        {
            get
            {
                if (Turnover1 == 0)
                {
                    return 0;
                }

                return Winloss1 / Turnover1;
            }
        }

        public decimal Turnover2 { get; set; }

        public decimal Winloss2 { get; set; }

        public decimal Margin2
        {
            get
            {
                if (Turnover2 == 0)
                {
                    return 0;
                }

                return Winloss2 / Turnover2;
            }
        }

        public decimal Turnover3 { get; set; }

        public decimal Winloss3 { get; set; }

        public decimal Margin3
        {
            get
            {
                if (Turnover3 == 0)
                {
                    return 0;
                }

                return Winloss3 / Turnover3;
            }
        }

        public string Evidence { get; set; }

        public string BreakColumn { get; set; } = string.Empty;

        public int CustId { get; set; }

        public bool IsRootMember { get; set; } = true;

        public string AssociatedAccount { get; set; }

        public int Level0CustId { get; set; }
    }
}