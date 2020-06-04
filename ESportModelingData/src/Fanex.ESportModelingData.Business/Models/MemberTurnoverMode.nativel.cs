using CsvHelper.Configuration.Attributes;

namespace Fanex.ESportModelingData.Business.Models
{
    public class MemberTurnoverModel
    {
        [Name("Customer Name")]
        public string Customer { get; set; }

        [Name("Member Turnover")]
        public decimal MemberTurnover { get; set; }
    }
}