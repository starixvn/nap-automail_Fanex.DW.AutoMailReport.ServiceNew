using System;
using System.Configuration;
using Fanex.Common;

namespace FastMarketWeeklyPerformance.Business.Services
{
    public class DateRangeByManualImp : DateRange
    {
        public DateTime FromDate => Converter.To<DateTime>(ConfigurationManager.AppSettings["FromDate"]);

        public DateTime ToDate => Converter.To<DateTime>(ConfigurationManager.AppSettings["ToDate"]);
    }
}