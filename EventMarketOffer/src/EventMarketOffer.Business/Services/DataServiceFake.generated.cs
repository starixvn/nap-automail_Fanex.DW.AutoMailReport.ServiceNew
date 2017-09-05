using System;
using System.Data;
using System.IO;
using Fanex.Service.Core;
using Newtonsoft.Json;

namespace EventMarketOffer.Business.Services
{
    public class DataServiceFake : DataService, RepositoryService
    {
        public bool IsDataAvailable()
        {
            return true;
        }

        public DataTable Query()
        {
            var text = File.ReadAllText(@"Templates\DataSource.json");
            return JsonConvert.DeserializeObject<DataTable>(text);
        }

        public DateTime QueryLastDate()
        {
            return SystemTime.Now.AddDays(-1);
        }
    }
}