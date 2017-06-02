namespace BettypeWith3rdProducts.Business.Services
{
    public class ViewOfDataBySummaryServiceImp : ViewOfDataService
    {
        private int? sportId;

        public int? PeriodId
        {
            get { return null; }
        }        

        public int? SportId
        {
            get { return null; }
            set { sportId = value; }
        }
    }
}