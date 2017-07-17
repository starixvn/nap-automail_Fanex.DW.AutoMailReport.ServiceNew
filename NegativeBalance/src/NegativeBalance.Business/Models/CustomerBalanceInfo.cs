using System;

namespace NegativeBalance.Business.Models
{
    public class CustomerBalanceInfo
    {
        public int LogId { get; set; }

        public DateTime LogDate { get; set; }

        public int MatchId { get; set; } 

        public int GroupId { get; set; }

        public string CustId { get; set; } 

        public string UserName { get; set; }

        public double TotalBalance { get; set; }

        public string CurrencyName { get; set; } 

        public string Site { get; set; }

        public string CustomInfo1 { get; set; } 
    }
}