using Newtonsoft.Json;

namespace Fanex.DHWL.Business.Models.Json
{
    public class CategoryModel
    {
        [JsonProperty("categ")] public string Category { get; set; }

        [JsonProperty("p")] public decimal HouseWl { get; set; }

        [JsonProperty("r")] public decimal HouseTo { get; set; }

        [JsonProperty("cre")] public TowlModel Credit { get; set; }

        [JsonProperty("lic")] public TowlModel Licensee { get; set; }

        [JsonProperty("m_p")] public decimal MtdHouseWl { get; set; }

        [JsonProperty("m_r")] public decimal MtdHouseTo { get; set; }

        [JsonProperty("m_cre")] public TowlModel MtdCredit { get; set; }

        [JsonProperty("m_lic")] public TowlModel MtdLicensee { get; set; }
    }
}