using Fanex.ESportModelingData.Business.Models;
using System;
using System.Collections.Generic;

namespace Fanex.ESportModelingData.Business.Services
{
    public interface GenerateTemplateProvider
    {
        GenerateTemplate Execute(Period period, DateTime beginDate);
    }

    public class GenerateTemplateProviderImp : GenerateTemplateProvider
    {
        private readonly DateTimeServiceProvider dateService;

        public GenerateTemplateProviderImp(DateTimeServiceProvider dateService)
        {
            this.dateService = dateService;
        }

        public GenerateTemplate Execute(Period period, DateTime beginDate)
        {
            var date = dateService.Execute(period, beginDate);
            var templates = new Dictionary<Period, GenerateTemplate>
            {
                { Period.Daily, new GenerateTemplateDailyImp (date) },
                { Period.Weekly, new GenerateTemplateWeeklymp (date) },
                { Period.Manual, new GenerateTemplateWeeklymp (date) },
                { Period.DailyHistoricalData, new GenerateTemplateDailyHistoryImp (date) },
                { Period.WeeklyHistoricalData, new GenerateTemplateWeeklyHistoryImp (date) },
            };

            return templates[period];
        }
    }
}