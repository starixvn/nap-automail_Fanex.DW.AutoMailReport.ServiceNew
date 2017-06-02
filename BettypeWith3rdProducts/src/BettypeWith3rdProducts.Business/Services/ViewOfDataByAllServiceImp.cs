namespace BettypeWith3rdProducts.Business.Services
{
    public class ViewOfDataByAllServiceImp : ViewOfDataService
    {
        private int? sportId;

        public int? PeriodId
        {
            get { return 1; }
        }

        public int? SportId
        {
            get { return null; }
            set { sportId = value; }
        }
    }
}