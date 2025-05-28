using Newtonsoft.Json;
using System.Collections.Generic;

namespace Fanex.DHWL.Business.Models.Json
{
    public class TypeModel
    {
        [JsonProperty("type")] public string Type { get; set; }

        [JsonProperty("data")] public List<CategoryModel> Categories { get; set; }
    }
}