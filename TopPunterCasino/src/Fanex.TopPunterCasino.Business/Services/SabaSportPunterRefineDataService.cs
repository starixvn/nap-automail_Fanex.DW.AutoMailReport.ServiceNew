using Fanex.SabaSoccerLetstalk.Business.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Fanex.SabaSoccerLetstalk.Business.Services
{
    public interface SabaSportPunterRefineDataService
    {
        void StoreRawData(Sport sport, DateTime date, List<PunterModel> rawData);

        List<PunterModel> LoadRefineData(Sport sport, DateTime date, List<PunterModel> rawData);
    }

    public class SabaSportPunterRefineDataServiceImp : SabaSportPunterRefineDataService
    {
        public List<PunterModel> LoadRefineData(Sport sport, DateTime date, List<PunterModel> rawData)
        {
            var previousRawData = LoadPreviousRawData(sport, date);
            var result = new List<PunterModel>();

            if (previousRawData.Count == 0)
            {
                return rawData;
            }

            rawData.ForEach(row =>
            {
                var previousRow = previousRawData.FirstOrDefault(s => s.Member == row.Member);
                if (previousRow == null)
                {
                    result.Add(row);
                }
                else if (row.BetCount > previousRow.BetCount)
                {
                    row.IsSetBGColor = true;
                    result.Add(row);
                }
            });

            return result;
        }

        public void StoreRawData(Sport sport, DateTime date, List<PunterModel> rawData)
        {
            var path = $@"{AppConfigService.Config.InternalPath}\{date:yyyy}\{date:MM}";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            path = $@"{path}\{sport}_{date:yyyyMMdd}.json";
            var json = JsonConvert.SerializeObject(rawData, Formatting.Indented);
            File.WriteAllText(path, json);
        }

        private List<PunterModel> LoadPreviousRawData(Sport sport, DateTime date)
        {
            var path = $@"{AppConfigService.Config.InternalPath}\{date:yyyy}\{date:MM}";
            path = $@"{path}\{sport}_{date:yyyyMMdd}.json";
            if (!File.Exists(path))
            {
                return new List<PunterModel>();
            }

            var json = File.ReadAllText(path);

            return JsonConvert.DeserializeObject<List<PunterModel>>(json);
        }
    }
}