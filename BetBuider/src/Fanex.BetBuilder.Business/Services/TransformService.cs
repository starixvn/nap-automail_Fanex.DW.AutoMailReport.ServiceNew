using System.Collections.Generic;
using System.Linq;
using Fanex.BetBuilder.Business.Models;

namespace Fanex.BetBuilder.Business.Services
{
    public interface TransformService
    {
        List<DailyDataModel> ExecuteDaily(List<DailyRawDataModel> rawData);

        List<MonthlyDataModel> ExecuteMonthly(List<MonthlyRawDataModel> rawData);
    }

    public class TransformServiceImp : TransformService
    {
        private readonly CacheRefService cacheRefService;

        public TransformServiceImp(CacheRefService cacheRefService)
        {
            this.cacheRefService = cacheRefService;
        }

        public List<DailyDataModel> ExecuteDaily(List<DailyRawDataModel> rawData)
        {
            var data = rawData.Take(rawData.Count - 1).ToList();
            var grandTotalRow = rawData[rawData.Count - 1];
            var result = new List<DailyDataModel>();
            data.ForEach(r =>
            {
                DailyDataModel model = MappingDailyData(r);
                result.Add(model);
            });

            result = result.OrderBy(s => s.LeagueName)
                .ThenBy(s => s.FixtureName)
                .ThenBy(s => s.MarketId)
                .ToList();

            var grandTotal = MappingDailyData(grandTotalRow);
            result.Add(grandTotal);

            return result;
        }

        public List<MonthlyDataModel> ExecuteMonthly(List<MonthlyRawDataModel> rawData)
        {
            var result = new List<MonthlyDataModel>();
            rawData.ForEach(r =>
            {
                MonthlyDataModel model = MappingMonthlyData(r);
                result.Add(model);
            });

            result = result.OrderBy(s => s.LeagueName)
                .ThenBy(s => s.FixtureName)
                .ThenBy(s => s.MarketId)
                .ToList();

            return result;
        }

        private DailyDataModel MappingDailyData(DailyRawDataModel r)
        {
            var fixtureName = cacheRefService.GetTeamName(r.HomeId.ToString()) + " vs " + cacheRefService.GetTeamName(r.AwayId.ToString());
            fixtureName = (r.HomeId == 0 && r.AwayId == 0) ? string.Empty : fixtureName;
            return new DailyDataModel
            {
                Sport = r.Sport,
                LeagueName = cacheRefService.GetLeagueName(r.LeagueId.ToString()),
                FixtureName = fixtureName,
                BetDescription = r.BetDescription,
                GlobalShowTime = r.GlobalShowTime,
                MarketId = r.MarketId,
                DateTimePlaced = r.DateTimePlaced,
                NoOfLegsPerBet = r.NoOfLegsPerBet,
                BetStatus = r.BetStatus,
                DeviceGroup = r.DeviceGroup,
                BetPrice = r.BetPrice,
                Code = r.Code,
                Stake = r.Stake,
                Result = r.Result,
                Payout = r.Payout,
                Commission = r.Commission,
                GrossRevenue = r.GrossRevenue
            };
        }

        private MonthlyDataModel MappingMonthlyData(MonthlyRawDataModel r)
        {
            var fixtureName = cacheRefService.GetTeamName(r.HomeId.ToString()) + " vs " + cacheRefService.GetTeamName(r.AwayId.ToString());
            fixtureName = (r.HomeId == 0 && r.AwayId == 0) ? string.Empty : fixtureName;
            return new MonthlyDataModel
            {
                Sport = r.Sport,
                LeagueName = cacheRefService.GetLeagueName(r.LeagueId.ToString()),
                FixtureName = fixtureName,
                TransId = r.TransId,
                BetDescription = r.BetDescription,
                GlobalShowTime = r.GlobalShowTime,
                MarketId = r.MarketId,
                DateTimePlaced = r.DateTimePlaced,
                WinLostDate = r.WinLostDate,
                NoOfLegsPerBet = r.NoOfLegsPerBet,
                BetStatus = r.BetStatus,
                DeviceGroup = r.DeviceGroup,
                BetPrice = r.BetPrice,
                Code = r.Code,
                StakeBefore = r.StakeBefore,
                StakeAfter = r.StakeAfter,
                ResultBefore = r.ResultBefore,
                ResultAfter = r.ResultAfter,
                PayoutBefore = r.PayoutBefore,
                PayoutAfter = r.PayoutAfter,
                CommissionBefore = r.CommissionBefore,
                CommissionAfter = r.CommissionAfter,
                GrossRevenueBefore = r.GrossRevenueBefore,
                GrossRevenueAfter = r.GrossRevenueAfter
            };
        }
    }
}