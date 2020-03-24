using Fanex.AbnormalBets.Business.Models.Headline;
using Fanex.RemotingRef;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Fanex.AbnormalBets.Business.Services
{
    public interface TransformHeadlineService
    {
        Tuple<
           List<SummaryBetDTOModel>,
           List<DetailBetDTOModel>,
           List<UserLocationModel>> ExecuteDailyLicensee(Tuple<
               List<SummaryBetModel>,
               List<DetailBetModel>,
               List<UserLocationModel>> rawData);
    }

    public class TransformHeadlineServiceImp : TransformHeadlineService
    {
        private readonly IRemotingRefService rrService;
        private readonly BetteamService betteamService;
        private readonly RepositoryHeadlineService repositoryService;

        private readonly string CodeEn = "en-US";
        private readonly string CodeCn = "zh-CN";

        public TransformHeadlineServiceImp(
            IRemotingRefService rrService,
            BetteamService betteamService,
            RepositoryHeadlineService repositoryService)
        {
            this.rrService = rrService;
            this.betteamService = betteamService;
            this.repositoryService = repositoryService;
        }

        public Tuple<
            List<SummaryBetDTOModel>,
            List<DetailBetDTOModel>,
            List<UserLocationModel>> ExecuteDailyLicensee(Tuple<
                List<SummaryBetModel>,
                List<DetailBetModel>,
                List<UserLocationModel>> rawData)
        {
            var summaries = GetSumaryBets(rawData.Item1);
            var details = GetDetailBets(rawData.Item2);
            var locations = GetLocation(rawData.Item3);

            return new Tuple<
                List<SummaryBetDTOModel>,
                List<DetailBetDTOModel>,
                List<UserLocationModel>>(summaries, details, locations);
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
                    Live = bet.LiveIndicator ? "Live 现场" : "Non Live  非现场",
                    NumberOfLicensees = bet.NumberOfLicensees,
                    NumberOfTickets = bet.NumberOfTickets,
                    Bettype = $"{rrService.GetBetTypeName(bet.Bettype.ToString(), CodeEn)} {rrService.GetBetTypeName(bet.Bettype.ToString(), CodeCn)}",
                    Betteam = betteamService.GetBetteam(bet.Bettype, bet.Betteam),
                    Reason = bet.Reason,
                    FinalScore = $"{bet.FinalHomeScore}-{bet.FinalAwayScore}",                    
                });
            });

            return summaries
                .OrderByDescending(o => o.KickOffTime)
                .ThenByDescending(o => o.Live)
                .ThenByDescending(o => o.Bettype)
                .ToList();
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
                    KickOffTime = bet.KickOffTime.AddHours(12),
                    Live = bet.LiveIndicator ? "Live 现场" : "Non Live  非现场",
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
                    TransId = bet.TransId,
                    Ip = bet.Ip,
                    Location = GetLocationFromIp(bet.Ip),
                    Super = bet.Super,
                    Master = bet.Master,
                    Agent = bet.Agent
                });
            });

            return details
                .OrderByDescending(o => o.TransDate)
                .ToList();
        }

        private List<UserLocationModel> GetLocation(List<UserLocationModel> rawData)
        {
            var locations = new List<UserLocationModel>();
            rawData.ForEach(location =>
            {
                locations.Add(new UserLocationModel
                {
                    Username = location.Username,
                    IpLogin = location.IpLogin,
                    LastLogionLocation = GetLocationFromIp(location.IpLogin),
                    TicketIps = location.TicketIps,
                    TicketLocationIp = GetLocationFromIp(location.TicketIps),
                    Super = location.Super,
                    Master = location.Master,
                    Agent = location.Agent
                });
            });

            return locations;
        }

        private string GetLocationFromIp(string ip)
        {
            if (string.IsNullOrEmpty(ip))
            {
                return string.Empty;
            }

            var locations = new List<string>();
            ip.Split(',').ToList().ForEach(i =>
            {
                var location = repositoryService.QueryIP(i);
                var list = new List<string>
                {
                    location.CountryLong,
                    location.IpRegion,
                    location.IpCity
                };

                locations.Add(string.Join(", ", list.Where(s => s.Length > 2)));
            });

            return string.Join("; ", locations);
        }
    }
}