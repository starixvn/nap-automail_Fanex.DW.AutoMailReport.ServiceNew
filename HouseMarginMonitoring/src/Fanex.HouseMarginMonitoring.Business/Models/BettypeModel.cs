namespace Fanex.HouseMarginMonitoring.Business.Models
{
    public class BettypeModel
    {
        public int Order { get; set; }
        public string BettypeName { get; set; }

        public decimal HouseWL { get; set; }

        public decimal HouseTO { get; set; }

        public decimal Margin
        {
            get
            {
                if (HouseTO == 0)
                {
                    return 0;
                }

                return HouseWL / HouseTO;
            }
        }
    }
}