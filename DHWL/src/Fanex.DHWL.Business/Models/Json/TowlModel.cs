using Newtonsoft.Json;

namespace Fanex.DHWL.Business.Models.Json
{
    public class TowlModel
    {
        [JsonProperty("r")] public decimal HouseTo { get; set; }
        [JsonProperty("p")] public decimal HouseWl { get; set; }
    }
}