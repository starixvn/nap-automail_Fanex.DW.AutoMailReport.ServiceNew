using Fanex.AbnormalBets.Business.Models.CSD;
using Fanex.AbnormalBets.Business.Models.DailyCSD;
using Fanex.RemotingRef;
using System.Collections.Generic;
using System.Linq;

namespace Fanex.AbnormalBets.Business.Services
{
    public interface TransformCSDService
    {
        List<AbnormalBetDTOModel> ExecuteDailyCSD(List<AbnormalBetDailyModel> rawData);
    }

    public class TransformCSDServiceImp : TransformCSDService
    {
        private readonly IRemotingRefService rrService;
        private readonly string CodeEn = "en-US";

        public TransformCSDServiceImp(IRemotingRefService rrService)
        {
            this.rrService = rrService;
        }

        public List<AbnormalBetDTOModel> ExecuteDailyCSD(List<AbnormalBetDailyModel> rawData)
        {
            var result = new List<AbnormalBetDTOModel>();
            rawData.ForEach(bet =>
            {
                result.Add(new AbnormalBetDTOModel
                {
                    Date = bet.Date,
                    Sport = rrService.GetSportName(bet.SportType.ToString(), CodeEn),
                    League = rrService.GetLeagueName(bet.Leagueid.ToString(), CodeEn),
                    Match = $"{rrService.GetTeamName(bet.HomeId.ToString(), CodeEn)} -vs- {rrService.GetTeamName(bet.AwayId.ToString(), CodeEn)}",
                    GlobalShowTime = bet.GlobalShowTime.AddHours(12),
                    Live = bet.Live,
                    NonLive = bet.NonLive,
                    TotalOfTickets = bet.TotalOfTickets,
                    Sites = bet.Sites,
                    AffectedSite = bet.AffectedSite,
                    Bettypes = GetBettype(bet.Bettypes),
                    Reason = bet.Reason,
                    HTScore = $"{bet.HtHomeScore}-{bet.HtAwayScore}",
                    FinalScore = $"{bet.FinalHomeScore}-{bet.FinalAwayScore}",
                    Username = bet.UserName,
                    CreatedDate = bet.CreatedDate
                });
            });

            return result
                .OrderByDescending(o => o.GlobalShowTime)
                .ToList();
        }

        private string GetBettype(string bettypes)
        {
            var result = new List<string>();
            bettypes.Split(',')
                .ToList()
                .ForEach(bettype =>
                {
                    result.Add($"{rrService.GetBetTypeName(bettype, CodeEn)}");
                });

            return string.Join(", ", result.OrderBy(o => o.ToString()));
        }
    }
}