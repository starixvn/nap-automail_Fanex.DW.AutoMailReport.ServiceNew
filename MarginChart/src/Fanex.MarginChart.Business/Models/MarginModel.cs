namespace Fanex.MarginChart.Business.Models
{
    public class MarginModel
    {
        public string Site { get; set; }

        public decimal HWL { get; set; }

        public decimal TO { get; set; }

        public decimal Margin
        {
            get
            {
                if (TO == 0)
                {
                    return 0;
                }

                return HWL / TO;
            }
        }
    }
}