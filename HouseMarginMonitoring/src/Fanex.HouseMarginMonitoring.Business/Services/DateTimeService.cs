using System;

namespace Fanex.HouseMarginMonitoring.Business.Services
{
    public interface DateTimeService
    {
        DateTime FromDate { get; }

        DateTime ToDate { get; }
    }
}