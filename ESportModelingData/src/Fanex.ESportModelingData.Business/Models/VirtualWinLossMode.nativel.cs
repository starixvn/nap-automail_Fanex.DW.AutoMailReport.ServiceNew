using CsvHelper.Configuration.Attributes;

namespace Fanex.ESportModelingData.Business.Models
{
    public class VirtualWinLossModel
    {
        [Name("Customer Name")]
        public string Customer { get; set; }

        [Name("Virtual Win Loss")]
        public decimal VirtualWinLoss { get; set; }
    }
}