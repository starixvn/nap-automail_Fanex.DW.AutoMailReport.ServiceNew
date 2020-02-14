using System;

namespace Fanex.MailLetstalkTemplate.Business.Services
{
    public interface DateRangeService
    {
        DateTime FromDate { get; }

        DateTime ToDate { get; }
    }
}