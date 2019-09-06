using System;

namespace Fanex.SoccerPerformance.Business.Services
{
    public interface DateService
    {
        DateTime Yesterday { get; }

        DateTime StartDate { get; }

        DateTime EndDate { get; }
    }
}
