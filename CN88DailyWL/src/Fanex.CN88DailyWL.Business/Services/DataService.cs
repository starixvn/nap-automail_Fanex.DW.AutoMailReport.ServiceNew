using Fanex.CN88DailyWL.Business.Criterias;
using Fanex.CN88DailyWL.Business.Models;
using Fanex.Data.Repository;
using System;

namespace Fanex.CN88DailyWL.Business.Services
{
    public interface DataService
    {
        Tuple<CN88Model, CN88Model> QueryData();
    }

    public class DataServiceImp : DataService
    {
        private readonly IDynamicRepository dynamicRepository;
        private readonly DateService dateService;

        public DataServiceImp(IDynamicRepository dynamicRepository, DateService dateService)
        {
            this.dynamicRepository = dynamicRepository;
            this.dateService = dateService;
        }

        public Tuple<CN88Model, CN88Model> QueryData()
        {
            var date = GetPerid();
            var fromdate = dateService.WinLostDate;
            var todate = dateService.WinLostDate;
            var todayData = GetData(fromdate, todate);
            var periodData = GetData(date.FromDate, date.ToDate);
            if (date.FromDate == date.ToDate)
            {
                periodData.Date = $"{date.FromDate:yyyyMMdd} - {date.ToDate:yyyyMMdd}";
            }

            return new Tuple<CN88Model, CN88Model>(todayData, periodData);
        }

        private CN88Model GetData(DateTime fromDate, DateTime toDate)
        {
            var criteria = new CN88Criteria { FromDate = fromDate, ToDate = toDate };
            return dynamicRepository.Get<CN88Model>(criteria);
        }

        private DateTimeModel GetPerid()
        {
            var period = dynamicRepository.Get<DateTimeModel>(new GetPeriodCriteria { RptDate = dateService.WinLostDate });
            if (AppConfigService.AppConfig.IsManual)
            {
                period.ToDate = dateService.WinLostDate;
            }

            return period;
        }
    }
}