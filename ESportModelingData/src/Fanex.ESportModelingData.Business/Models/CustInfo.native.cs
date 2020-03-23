using CsvHelper.Configuration.Attributes;

namespace Fanex.ESportModelingData.Business.Models
{
    public class CustInfo
    {
        public string Super { get; set; }

        public string Master { get; set; }

        public string Agent { get; set; }

        [Name("Customer Name")]
        public string CustomerName { get; set; }

        public bool Root { get; set; }
    }
}