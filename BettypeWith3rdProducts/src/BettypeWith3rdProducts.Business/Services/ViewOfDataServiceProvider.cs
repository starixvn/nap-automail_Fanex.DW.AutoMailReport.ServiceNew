using System.Collections.Generic;
using BettypeWith3rdProducts.Business.Models;

namespace BettypeWith3rdProducts.Business.Services
{
    public interface ViewOfDataServiceProvider
    {
        ViewOfDataService View(ViewOfData viewOfData, int? sportId);
    }

    public class ViewOfDataServiceProviderImp : ViewOfDataServiceProvider
    {
        private int? sportId;

        private Dictionary<ViewOfData, ViewOfDataService> PointOfView
        {
            get
            {
                return new Dictionary<ViewOfData, ViewOfDataService>
                {
                    { ViewOfData.All, new ViewOfDataByAllServiceImp() },
                    { ViewOfData.Detail, new ViewOfDataByDetailServiceImp(sportId) },
                    { ViewOfData.Summary, new ViewOfDataBySummaryServiceImp() },
                };
            }
        }

        public ViewOfDataService View(ViewOfData viewOfData, int? sportId)
        {
            this.sportId = sportId;
            return PointOfView[viewOfData];
        }
    }
}