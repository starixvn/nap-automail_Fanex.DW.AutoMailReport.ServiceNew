using System.Collections.Generic;
using BettypeWith3rdProducts.Business.Models;

namespace BettypeWith3rdProducts.Business.Services
{
    public interface ParameterService
    {
        Parameter GetParameter(ViewOfData viewOfData, int? sportId);
    }

    public class ParameterServiceImp : ParameterService
    {
        private int? sportId;

        private Dictionary<ViewOfData, Parameter> Parameters
        {
            get
            {
                return new Dictionary<ViewOfData, Parameter>
                {
                    { ViewOfData.All, new Parameter { SportId = null, PeriodId = 1 } },
                    { ViewOfData.Detail, new Parameter { SportId = sportId, PeriodId = 1 } },
                    { ViewOfData.Summary, new Parameter { SportId = null, PeriodId = null } },
                };
            }
        }

        public Parameter GetParameter(ViewOfData viewOfData, int? sportId)
        {
            this.sportId = sportId;
            return Parameters[viewOfData];
        }
    }
}