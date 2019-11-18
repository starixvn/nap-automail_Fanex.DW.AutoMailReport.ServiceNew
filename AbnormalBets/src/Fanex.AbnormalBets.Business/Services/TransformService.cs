using Fanex.AbnormalBets.Business.Models;
using Fanex.RemotingRef;
using System;
using System.Collections.Generic;

namespace Fanex.AbnormalBets.Business.Services
{
    public interface TransformService
    {
        Tuple<List<SummaryBetDTOModel>, List<DetailBetDTOModel>> Execute(Tuple<List<SummaryBetModel>, List<DetailBetModel>> rawData);
    }

    public class TransformServiceImp : TransformService
    {
        private readonly IRemotingRefService rrService;
        private readonly BetteamService betteamService;

        private readonly string CodeEn = "en-US";
        private readonly string CodeCn = "zh-CN";

        public TransformServiceImp(IRemotingRefService rrService, BetteamService betteamService)
        {
            this.rrService = rrService;
            this.betteamService = betteamService;
        }

        public Tuple<List<SummaryBetDTOModel>, List<DetailBetDTOModel>> Execute(Tuple<List<SummaryBetModel>, List<DetailBetModel>> rawData)
        {
            var summaries = GetSumaryBets(rawData.Item1);
            var details = GetDetailBets(rawData.Item2);

            return new Tuple<List<SummaryBetDTOModel>, List<DetailBetDTOModel>>(summaries, details);
        }

        private List<SummaryBetDTOModel> GetSumaryBets(List<SummaryBetModel> rawData)
        {
            var summaries = new List<SummaryBetDTOModel>();
            rawData.ForEach(bet =>
            {
                summaries.Add(new SummaryBetDTOModel
                {
                    Sport = $"{rrService.GetSportName(bet.SportType.ToString(), CodeEn)} {rrService.GetSportName(bet.SportType.ToString(), CodeCn)}",
                    League = $"{rrService.GetLeagueName(bet.LeagueId.ToString(), CodeEn)} {rrService.GetLeagueName(bet.LeagueId.ToString(), CodeCn)}",
                    Match = $"{rrService.GetTeamName(bet.HomeId.ToString(), CodeEn)} -vs- {rrService.GetTeamName(bet.AwayId.ToString(), CodeEn)} {rrService.GetTeamName(bet.HomeId.ToString(), CodeCn)} -vs- {rrService.GetTeamName(bet.AwayId.ToString(), CodeCn)}",                    
                    KickOffTime = bet.KickOffTime.AddHours(12),
                    Live = bet.LiveIndicator ? "Live 现场" : "Non Live  现场",
                    NumberOfLicensees = bet.NumberOfLicensees,
                    NumberOfTickets = bet.NumberOfTickets,
                    Bettype = $"{rrService.GetBetTypeName(bet.Bettype.ToString(), CodeEn)} {rrService.GetBetTypeName(bet.Bettype.ToString(), CodeCn)}",
                    Betteam = betteamService.GetBetteam(bet.Bettype, bet.Betteam),
                    Reason = bet.Reason,
                    FinalScore = $"{bet.FinalHomeScore}-{bet.FinalAwayScore}"
                });
            });

            return summaries;
        }

        private List<DetailBetDTOModel> GetDetailBets(List<DetailBetModel> rawData)
        {
            var details = new List<DetailBetDTOModel>();
            rawData.ForEach(bet =>
            {
                details.Add(new DetailBetDTOModel
                {
                    Sport = $"{rrService.GetSportName(bet.SportType.ToString(), CodeEn)} {rrService.GetSportName(bet.SportType.ToString(), CodeCn)}",
                    League = $"{rrService.GetLeagueName(bet.LeagueId.ToString(), CodeEn)} {rrService.GetLeagueName(bet.LeagueId.ToString(), CodeCn)}",
                    Match = $"{rrService.GetTeamName(bet.HomeId.ToString(), CodeEn)} -vs- {rrService.GetTeamName(bet.AwayId.ToString(), CodeEn)} {rrService.GetTeamName(bet.HomeId.ToString(), CodeCn)} -vs- {rrService.GetTeamName(bet.AwayId.ToString(), CodeCn)}",
                    Home = $"{rrService.GetTeamName(bet.HomeId.ToString(), CodeEn)}",
                    KickOffTime = bet.KickOffTime.AddHours(12),
                    Live = bet.LiveIndicator ? "Live 现场" : "Non Live  现场",
                    Bettype = $"{rrService.GetBetTypeName(bet.Bettype.ToString(), CodeEn)} {rrService.GetBetTypeName(bet.Bettype.ToString(), CodeCn)}",
                    Betteam = betteamService.GetBetteam(bet.Bettype, bet.Betteam),
                    Reason = bet.Reason,
                    FinalScore = $"{bet.FinalHomeScore}-{bet.FinalAwayScore}",
                    Username = bet.Username,
                    Odds = bet.Odds,
                    CurrencyName = bet.CurrencyName,
                    Stake = bet.Stake,
                    Hdp = bet.Hdp,
                    LiveHomeScore = bet.LiveHomeScore,
                    LiveAwayScore = bet.LiveAwayScore,
                    TransDate = bet.TransDate,
                    TransId = bet.TransId
                });
            });

            return details;
        }
    }
}