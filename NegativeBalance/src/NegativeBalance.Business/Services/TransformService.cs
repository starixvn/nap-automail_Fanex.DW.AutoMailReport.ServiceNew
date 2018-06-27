using System.Collections.Generic;
using Fanex.Logging;
using NegativeBalance.Business.Models;
using Newtonsoft.Json;
using RemotingRef;

namespace NegativeBalance.Business.Services
{
    public interface TransformService
    {
        MatchInfo Execute(string licenseeName, RawMatchInfo rawMatchInfo, int groupId);
    }

    public class TransformServiceImp : TransformService
    {
        private readonly string langEN = "en";
        private readonly string langCS = "cs";

        public MatchInfo Execute(string licenseeName, RawMatchInfo rawMatchInfo, int groupId)
        {
            if (rawMatchInfo == null)
            {
                return new MatchInfo();
            }

            var matchInfo = new MatchInfo
            {
                GroupId = groupId,

                MatchId = rawMatchInfo.MatchId,
                MatchCode = rawMatchInfo.MatchCode,
                EventDate = rawMatchInfo.EventDate,
                LeagueName = RefData.GetLeagueName(rawMatchInfo.LeagueId.ToString(), langEN),
                LeagueNameCS = RefData.GetLeagueName(rawMatchInfo.LeagueId.ToString(), langCS),
                HomeName = RefData.GetTeamName(rawMatchInfo.HomeId.ToString(), langEN),
                HomeNameCS = RefData.GetTeamName(rawMatchInfo.HomeId.ToString(), langCS),
                AwayName = RefData.GetTeamName(rawMatchInfo.AwayId.ToString(), langEN),
                AwayNameCS = RefData.GetTeamName(rawMatchInfo.AwayId.ToString(), langCS),
                SportName = RefData.GetSportName(rawMatchInfo.SportId.ToString(), langEN),
                SportNameCS = RefData.GetSportName(rawMatchInfo.SportId.ToString(), langCS)                
            };

            WriteLog(licenseeName, rawMatchInfo, matchInfo);

            return matchInfo;
        }

        private static void WriteLog(string licenseeName, RawMatchInfo rawMatchInfo, MatchInfo matchInfo)
        {
            if (!matchInfo.IsValid)
            {
                var data = new List<object> { rawMatchInfo, matchInfo };
                var json = JsonConvert.SerializeObject(data, Formatting.Indented);
                IBCFile.Logger.Log(JsonConvert.SerializeObject(json, Formatting.Indented));
                LogService.Error($"Negative Balance Report - Remoting Ref returned empty data that belongs to licensee: {licenseeName}: {json}");
            }
        }
    }
}