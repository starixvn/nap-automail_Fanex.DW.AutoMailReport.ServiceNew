using CsvHelper.Configuration.Attributes;

namespace Fanex.ESportModelingData.Business.Models
{
    public class VirtualrateSettingModel
    {
        [Name("Customer Name")]
        public string Customer { get; set; }

        [Name("Virtualrate Setting")]
        public decimal VirtualrateSetting { get; set; }
    }
}