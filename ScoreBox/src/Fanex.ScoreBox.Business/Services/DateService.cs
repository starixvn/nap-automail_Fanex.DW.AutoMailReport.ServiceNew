using System;

namespace Fanex.ScoreBox.Business.Services
{
    public interface DateService
    {
        DateTime FromDate { get; }

        DateTime ToDate { get; }
    }
}