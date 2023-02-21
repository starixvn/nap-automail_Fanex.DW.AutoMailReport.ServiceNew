namespace Fanex.DHWL.Business.Models
{
    public class ChartMtdFigureModel
    {
        public double Mtd { get; set; }

        public double PreviousMtd { get; set; }

        public string Diff
        {
            get
            {
                var diff = Mtd - PreviousMtd;
                var result = diff > 0
                    ? $"+{(diff / PreviousMtd) * 100:N}%"
                    : $"{(diff / PreviousMtd) * 100:N}%";

                return result;
            }
        }
    }
}