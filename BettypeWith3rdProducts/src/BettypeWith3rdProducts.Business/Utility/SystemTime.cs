using System;

namespace BettypeWith3rdProducts.Business.Utility
{
    public static class SystemTime
    {
        private static DateTime dateTime;

        public static DateTime Now
        {
            get
            {
                if (dateTime != DateTime.MinValue)
                {
                    return dateTime;
                }

                return DateTime.Now;
            }
        }

        public static void Set(DateTime customDateTime)
        {
            dateTime = customDateTime;
        }

        public static void Reset()
        {
            dateTime = DateTime.MinValue;
        }
    }
}