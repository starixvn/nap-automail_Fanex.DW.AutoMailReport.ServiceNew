using NegativeBalance.Business.Models;
using RemotingRef;

namespace NegativeBalance.Business.Services
{
    public interface TransformService
    {
        MatchInfo Execute(RawMatchInfo rawMatchInfo);
    }

    public class TransformServiceImp : TransformService
    {
        private readonly string langEN = "en";
        private readonly string langCS = "cs";

        public MatchInfo Execute(RawMatchInfo rawMatchInfo)
        {
            if (rawMatchInfo == null)
            {
                return new MatchInfo();
            }

            return new MatchInfo
            {
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
                SportNameCS = RefData.GetSportName(rawMatchInfo.SportId.ToString(), langCS),
            };
        }
    }
}