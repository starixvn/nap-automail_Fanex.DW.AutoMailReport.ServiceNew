using System;

namespace Fanex.SoccerNonLive.Business.Services
{
    public interface DateTimeService
    {
        DateTime FromDate { get; }

        DateTime ToDate { get; }
    }
}