using System;
using Fanex.Service.Core;

namespace Fanex.BetBuilder.Business.Services
{
    public interface CriteriaService
    {
        DateTime WinlostDate { get; }
    }

    public class CriteriaServiceImp : CriteriaService
    {
        public DateTime WinlostDate => AppConfigService.AppConfig.IsManual ? AppConfigService.AppConfig.WinlostDate : Convert.ToDateTime($"{SystemTime.Now.AddDays(-1):MM/dd/yyyy}");
    }
}