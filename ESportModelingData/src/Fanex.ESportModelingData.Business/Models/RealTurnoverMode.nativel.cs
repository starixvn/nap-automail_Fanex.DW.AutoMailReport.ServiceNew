using CsvHelper.Configuration.Attributes;

namespace Fanex.ESportModelingData.Business.Models
{
    public class RealTurnoverModel
    {
        [Name("Customer Name")]
        public string Customer { get; set; }

        [Name("Real Turnover")]
        public decimal RealTurnover { get; set; }
    }
}