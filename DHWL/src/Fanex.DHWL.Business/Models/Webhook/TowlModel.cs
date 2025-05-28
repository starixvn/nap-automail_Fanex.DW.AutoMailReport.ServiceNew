using Newtonsoft.Json;

namespace Fanex.DHWL.Business.Models.Webhook
{
    public class TowlModel
    {
        [JsonProperty("p")] public int HouseWl { get; set; }
        [JsonProperty("r")] public int HouseTo { get; set; }
    }
}