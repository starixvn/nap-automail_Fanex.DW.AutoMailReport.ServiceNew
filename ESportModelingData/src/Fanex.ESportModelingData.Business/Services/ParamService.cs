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
            var dates = new List<DateTimeModel>();
            var dateEnd = dateTimeService.DateEnd();
            while (dateEnd <= AppConfigService.AppConfig.DateEnd)
            {
                dateEnd = dateTimeService.DateEnd();
                if (dateEnd <= AppConfigService.AppConfig.DateEnd)
                {
                    dates.Add(new DateTimeModel { DateStart = dateTimeService.DateStart(), DateEnd = dateEnd });
                }

                AppConfigService.AppConfig.DateStart = dateEnd.AddDays(1);
            }

            return dates;
        }
    }
}