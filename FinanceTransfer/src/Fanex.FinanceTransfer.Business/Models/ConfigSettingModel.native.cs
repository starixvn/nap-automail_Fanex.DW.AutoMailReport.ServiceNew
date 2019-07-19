using System;

namespace Fanex.FinanceTransfer.Business.Models
{
    public class ConfigSettingModel
    {
        public string SentryClientKey { get; set; }

        public string CategorySentryLog { get; set; }

        public bool IsManual { get; set; }

        public DateTime MoveBetsDay { get; set; }

        public DateTime? TransferMonthDate { get; set; }

        public int MailFunctionId { get; set; }
    }
}