using CsvHelper.Configuration.Attributes;

namespace Fanex.ESportModelingData.Business.Models
{
    public class BetCountModel
    {
        [Name("Customer Name")]
        public string Customer { get; set; }

        [Name("Bet Count")]
        public int BetCount { get; set; }
    }
}