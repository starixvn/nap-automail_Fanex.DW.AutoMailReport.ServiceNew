using Newtonsoft.Json;

namespace Fanex.DHWL.Business.Models.Webhook
{
    public class CategoryModel
    {
        [JsonProperty("categ")] public string Category { get; set; }

        [JsonProperty("d")] public PerfModel DailyPerf { get; set; }

        [JsonProperty("m")] public PerfModel MtdPerf { get; set; }
    }
}