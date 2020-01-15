using System;

namespace Fanex.IBOPerformance.Service.Models
{
    public class DailyCreatedCustomerModel
    {
        public int Counted { get; set; }

        public string UserName { get; set; }

        public DateTime CreateDate { get; set; }
    }
}