namespace Fanex.SuspiciousAccount.Business.Models
{
    public class AccountModel
    {
        public int CustId { get; set; }

        public string UserName { get; set; }

        public int NotifyStatus1 { get; set; }

        public int NotifyStatus2 { get; set; }

        public decimal Credit { get; set; }

        public string Currency { get; set; }
    }
}