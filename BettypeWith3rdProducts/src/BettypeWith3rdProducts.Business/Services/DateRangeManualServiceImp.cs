using Fanex.Common;
using System;
using System.Configuration;

namespace BettypeWith3rdProducts.Business.Services
{
    public class DateRangeManualServiceImp : DateRangeService
    {
        public DateTime DateStart => Converter.To<DateTime>(ConfigurationManager.AppSettings["DateStart"]);

        public DateTime DateEnd => Converter.To<DateTime>(ConfigurationManager.AppSettings["DateEnd"]);
    }
}