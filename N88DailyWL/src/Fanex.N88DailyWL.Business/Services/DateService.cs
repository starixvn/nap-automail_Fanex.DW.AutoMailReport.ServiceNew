using Fanex.Common;
using Fanex.Data.Repository;
using Fanex.N88DailyWL.Business.Criterias;
using Fanex.Service.Core;
using System;

namespace Fanex.N88DailyWL.Business.Services
{
    public interface DateService
    {
        DateTime FromDate { get; }

        DateTime ToDate { get; }
    }

    public class DateServiceImp : DateService
    {
        private readonly IDynamicRepository repository;

        public DateServiceImp(IDynamicRepository repository)
        {
            this.repository = repository;
        }

        public DateTime FromDate
        {
            get
            {
                if (AppConfigService.AppConfig.IsManual)
                {
                    return AppConfigService.AppConfig.FromDate;
                }

                var date = SystemTime.Now.AddDays(-1);
                return Converter.To<DateTime>($"{date:yyyy-MM-01}");
            }
        }

        public DateTime ToDate
        {
            get
            {
                if (AppConfigService.AppConfig.IsManual)
                {
                    return AppConfigService.AppConfig.ToDate;
                }

                var date = SystemTime.Now.AddDays(-1);
                return Converter.To<DateTime>($"{date:yyyy-MM-dd}");
            }
        }

        private LastDateModel QueryLastDate()
        {
            return repository.Get<LastDateModel>(new GetDWStatusCriteria());
        }
    }
}