using CsvHelper.Configuration.Attributes;

namespace Fanex.ESportModelingData.Business.Models
{
    public class MemberWinLossModel
    {
        [Name("Customer Name")]
        public string Customer { get; set; }

        [Name("Member Win Loss")]
        public decimal MemberWinLoss { get; set; }
    }
}