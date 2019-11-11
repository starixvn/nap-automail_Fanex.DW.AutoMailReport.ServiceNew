using System;

namespace Fanex.ESportDataMining.Business.Models
{
    public class DateModel
    {
        public DateTime DateKey { get; set; }

        public string DateName { get; set; }

        public DateTime WeekKey { get; set; }

        public string WeekName { get; set; }

        public DateTime MonthKey { get; set; }

        public string MonthName { get; set; }

        public string MonthAbbr { get; set; }

        public int MonthInt { get; set; }

        public string ConsecYearMonth { get; set; }

        public int MonthOfQuarter { get; set; }

        public int QuarterOfYear { get; set; }

        public string QuarterName { get; set; }

        public int Year { get; set; }
    }
}