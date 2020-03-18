using CsvHelper.Configuration.Attributes;

namespace Fanex.ESportModelingData.Business.Models
{
    public class RealIncomeModel
    {
        [Name("Customer Name")]
        public string Customer { get; set; }

        [Name("Real Win Loss")]
        public decimal RealIncome { get; set; }
    }
}