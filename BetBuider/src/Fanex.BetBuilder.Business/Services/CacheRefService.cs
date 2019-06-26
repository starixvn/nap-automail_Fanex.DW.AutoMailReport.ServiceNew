using Fanex.RemotingRef;

namespace Fanex.BetBuilder.Business.Services
{
    public interface CacheRefService
    {
        string GetLeagueName(string leagueId);

        string GetTeamName(string teamId);
    }

    public class CacheRefServiceImp : CacheRefService
    {
        private readonly string langEN = "en";
        private readonly IRemotingRefService remotingRefService;

        public CacheRefServiceImp(IRemotingRefService remotingRefService)
        {
            this.remotingRefService = remotingRefService;
        }

        public string GetLeagueName(string leagueId)
        {
            return remotingRefService.GetLeagueName(leagueId, langEN);
        }

        public string GetTeamName(string teamId)
        {
            return remotingRefService.GetTeamName(teamId, langEN);
        }
    }
}