using System;
using System.Configuration;
using Fanex.Common;

namespace BettypeWith3rdProducts.Business.Services
{
    public class DateRangeManualServiceImp : DateRangeService
    {
        public DateTime DateStart
        {
            get
            {
                return Converter.To<DateTime>(ConfigurationManager.AppSettings["DateStart"]);
            }
        }

        public DateTime DateEnd
        {
            get
            {
                return Converter.To<DateTime>(ConfigurationManager.AppSettings["DateEnd"]);
            }
        }
    }
}