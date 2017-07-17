using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using BettypeWith3rdProducts.Business.Services;
using Newtonsoft.Json;

namespace BettypeWith3rdProducts.BusinessSubstitute
{
    public class DataServiceSubstitute : DataService
    {
        public Dictionary<string, Dictionary<string, List<DataTable>>> Execute()
        {
            var text = File.ReadAllText(@"Templates\BettypeProducts-All.json");
            return JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, List<DataTable>>>>(text);
        }

        public bool IsDataAvailable()
        {
            return true;
        }
    }
}