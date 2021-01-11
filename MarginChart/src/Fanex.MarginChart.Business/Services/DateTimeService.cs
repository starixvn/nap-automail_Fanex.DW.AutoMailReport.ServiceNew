using System;

namespace Fanex.MarginChart.Business.Services
{
    public interface DateTimeService
    {
        DateTime FromDate { get; }

        DateTime ToDate { get; }
    }
}