using Newtonsoft.Json;
using System.Collections.Generic;

namespace Fanex.DHWL.Business.Models.Webhook
{
    public class TypeModel
    {
        [JsonProperty("data")] public List<CategoryModel> Categories { get; set; }
        [JsonProperty("type")] public string Type { get; set; }
    }
}