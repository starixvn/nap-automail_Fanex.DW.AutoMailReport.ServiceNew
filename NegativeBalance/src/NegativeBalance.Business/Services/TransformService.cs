using System.Collections.Generic;
using Fanex.Logging;
using Fanex.RemotingRef;
using NegativeBalance.Business.Models;
using Newtonsoft.Json;

namespace NegativeBalance.Business.Services
{
    public interface TransformService
    {
        MatchInfo Execute(string licenseeName, RawMatchInfo rawMatchInfo, int groupId);
    }

    public class TransformServiceImp : TransformService
    {
        private readonly string langEN = "en-US";
        private readonly string langCS = "zh-CN";
        private readonly IRemotingRefService remotingRefService;

        public TransformServiceImp(IRemotingRefService remotingRefService)
        {
            this.remotingRefService = remotingRefService;
        }

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
                LeagueName = remotingRefService.GetLeagueName(rawMatchInfo.LeagueId.ToString(), langEN),
                LeagueNameCS = remotingRefService.GetLeagueName(rawMatchInfo.LeagueId.ToString(), langCS),
                HomeName = remotingRefService.GetTeamName(rawMatchInfo.HomeId.ToString(), langEN),
                HomeNameCS = remotingRefService.GetTeamName(rawMatchInfo.HomeId.ToString(), langCS),
                AwayName = remotingRefService.GetTeamName(rawMatchInfo.AwayId.ToString(), langEN),
                AwayNameCS = remotingRefService.GetTeamName(rawMatchInfo.AwayId.ToString(), langCS),
                SportName = remotingRefService.GetSportName(rawMatchInfo.SportId.ToString(), langEN),
                SportNameCS = remotingRefService.GetSportName(rawMatchInfo.SportId.ToString(), langCS)
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
                LogService.Error($"Negative Balance Report - Remoting Ref returned empty data that belongs to licensee: {licenseeName}: {json}");
            }
        }
    }
}