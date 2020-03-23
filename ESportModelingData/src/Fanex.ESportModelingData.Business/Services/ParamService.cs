using Fanex.ESportModelingData.Business.Models;
using Fanex.Service.Core;
using System;
using System.Collections.Generic;

namespace Fanex.ESportModelingData.Business.Services
{
    public interface ParamService
    {
        List<DateTimeModel> Dates();
    }

    public class ParamServiceImp : ParamService
    {
        private readonly DateTimeService dateTimeService;

        public ParamServiceImp(DateTimeService dateTimeService)
        {
            this.dateTimeService = dateTimeService;
        }

        public List<DateTimeModel> Dates()
        {
            var dates = new Dictionary<Period, List<DateTimeModel>>
            {
                { Period.Manual, GetDatesManually() },
                { Period.Daily, GetDatesDaily() },
                { Period.Weekly, GetDatesWeekly() },
            };

            return dates[AppConfigService.AppConfig.Period];
        }

        private List<DateTimeModel> GetDatesWeekly()
        {
            return new List<DateTimeModel>
            {
                new DateTimeModel
                {
                    DateStart = dateTimeService.DateStart(),
                    DateEnd = dateTimeService.DateEnd()
                }
            };
        }

        private List<DateTimeModel> GetDatesManually()
        {
            var dates = new List<DateTimeModel>
            {
                new DateTimeModel
                {
                    DateStart = AppConfigService.AppConfig.DateStart,
                    DateEnd = AppConfigService.AppConfig.DateEnd
                }
            };

            return dates;
        }

        private List<DateTimeModel> GetDatesDaily()
        {
            var yesterday = Convert.ToDateTime($"{SystemTime.Now.AddDays(-1):yyyy-MM-dd}");
            return new List<DateTimeModel>
            {
                new DateTimeModel
                {
                    DateEnd = yesterday,
                    DateStart = yesterday
                }
            };
        }
    }
}