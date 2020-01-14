using Fanex.CN88DailyWL.Business.Criterias;
using Fanex.CN88DailyWL.Business.Models;
using Fanex.Data.Repository;
using Newtonsoft.Json;
using System;
using System.Linq;

namespace Fanex.CN88DailyWL.Business.Services
{
    public interface DataService
    {
        Tuple<ReportModel, ReportModel> QueryData();
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

        public Tuple<ReportModel, ReportModel> QueryData()
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

            return new Tuple<ReportModel, ReportModel>(todayData, periodData);
        }

        public ReportModel GetData(DateTime fromDate, DateTime toDate)
        {
            var model = new ReportModel();

            var criteria = new CN88Criteria { FromDate = fromDate, ToDate = toDate };
            var rawData = dynamicRepository.Fetch<CN88Model>(criteria);

            if (rawData.Any())
            {
                model.TPG = rawData.FirstOrDefault(x => x.Headline == "TPG");
                model.XPG = rawData.FirstOrDefault(x => x.Headline == "XPG");
            }

            model.Date = model.TPG.Date;
            return model;

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