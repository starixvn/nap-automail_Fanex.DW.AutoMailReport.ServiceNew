using Newtonsoft.Json;

namespace Fanex.DHWL.Business.Models.Webhook
{
    public class PerfModel
    {
        [JsonProperty("c")] public TowlModel Credit { get; set; }

        [JsonProperty("l")] public TowlModel Licensee { get; set; }
    }
}