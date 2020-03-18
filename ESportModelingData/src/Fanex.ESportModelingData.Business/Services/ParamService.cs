using System.Collections.Generic;
using Fanex.ESportModelingData.Business.Models;

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
            return AppConfigService.AppConfig.IsManual ? GetDatesFromHistory() : GetDatesFromCurrentTime();
        }

        private List<DateTimeModel> GetDatesFromCurrentTime()
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

        private List<DateTimeModel> GetDatesFromHistory()
        {
            var dates = new List<DateTimeModel>
            {
                new DateTimeModel { DateStart = AppConfigService.AppConfig.DateStart, DateEnd =  AppConfigService.AppConfig.DateEnd }
            };

            return dates;
        }
    }
}