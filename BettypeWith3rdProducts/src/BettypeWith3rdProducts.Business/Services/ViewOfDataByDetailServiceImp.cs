namespace BettypeWith3rdProducts.Business.Services
{
    public class ViewOfDataByDetailServiceImp : ViewOfDataService
    {
        private int? sportId;

        public ViewOfDataByDetailServiceImp(int? sportId)
        {
            this.sportId = sportId;
        }

        public int? PeriodId
        {
            get { return 1; }
        }

        public int? SportId
        {
            get { return sportId; }
            set { sportId = value; }
        }
    }
}