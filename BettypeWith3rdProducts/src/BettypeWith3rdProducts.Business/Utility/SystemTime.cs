using System;

namespace BettypeWith3rdProducts.Business.Utility
{
    public static class SystemTime
    {
        private static DateTime dateTime;

        public static DateTime Now => dateTime != DateTime.MinValue ? dateTime : DateTime.Now;

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