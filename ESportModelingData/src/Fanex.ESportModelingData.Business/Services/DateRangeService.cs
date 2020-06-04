using Fanex.ESportModelingData.Business.Models;
using System;
using System.Collections.Generic;

namespace Fanex.ESportModelingData.Business.Services
{
    public interface DateRangeService
    {
        List<DateTimeModel> Dates(Period period);
    }

    public class DateRangeServiceImp : DateRangeService
    {
        private readonly DateTimeServiceProvider dateService;

        public DateRangeServiceImp(DateTimeServiceProvider dateService)
        {
            this.dateService = dateService;
        }

        public List<DateTimeModel> Dates(Period period)
        {
            var dates = new Dictionary<Period, List<DateTimeModel>>
            {
                { Period.Daily,  DateByPeriod() },
                { Period.Weekly,  DateByPeriod() },
                { Period.Manual,  DateByPeriod() },
                { Period.DailyHistoricalData,  DailyHistoryDates() },
                { Period.WeeklyHistoricalData,  WeeklyHistoryDates() }
            };

            return dates[period];
        }

        private List<DateTimeModel> DateByPeriod()
        {
            var date = dateService.Execute(AppConfigService.AppConfig.Period, DateTime.Today);
            return new List<DateTimeModel>
            {
                new DateTimeModel
                {
                    DateStart = date.DateStart,
                    DateEnd = date.DateEnd
                }
            };
        }

        private List<DateTimeModel> DailyHistoryDates()
        {
            var result = new List<DateTimeModel>();
            var dateStart = AppConfigService.AppConfig.DateStart;
            while (dateStart <= AppConfigService.AppConfig.DateEnd)
            {
                result.Add(new DateTimeModel
                {
                    DateStart = dateStart,
                    DateEnd = dateStart,
                });

                dateStart = dateStart.AddDays(1);
            }

            return result;
        }

        private List<DateTimeModel> WeeklyHistoryDates()
        {
            var result = new List<DateTimeModel>();
            var dateStart = AppConfigService.AppConfig.DateStart;
            while (dateStart < AppConfigService.AppConfig.DateEnd)
            {
                var h = new DateTimeServiceWeeklyHistoryImp(dateStart);
                result.Add(new DateTimeModel
                {
                    DateStart = h.DateStart,
                    DateEnd = h.DateEnd,
                });

                dateStart = h.DateEnd.AddDays(1);
            }

            return result;
        }
    }
}