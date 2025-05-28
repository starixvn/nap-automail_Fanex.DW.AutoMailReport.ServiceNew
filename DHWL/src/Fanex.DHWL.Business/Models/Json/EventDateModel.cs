using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Fanex.DHWL.Business.Models.Json
{
    public class EventDateModel
    {
        [JsonProperty("evDate")] public DateTime Date { get; set; }
        [JsonProperty("jsondata")] public List<TypeModel> Data { get; set; }
    }
}