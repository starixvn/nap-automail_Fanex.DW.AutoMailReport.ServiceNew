using Newtonsoft.Json;
using System.Collections.Generic;

namespace Fanex.DHWL.Business.Models.Webhook
{
    public class DailyPerfReportMtdModel
    {
        [JsonProperty("type")] public string Type { get; set; }
        [JsonProperty("jsondata")] public List<EventDateModel> Data { get; set; }
    }
}