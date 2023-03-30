namespace Fanex.DHWL.Business.Models
{
    public class ChartMtdFigureModel
    {
        public double Mtd { get; set; }

        public double PreviousMtd { get; set; }

        public string DiffMtd
        {
            get
            {
                var diff = Mtd - PreviousMtd;
                var result = diff > 0
                    ? $"+{(diff / PreviousMtd) * 100:#,##0.00}%"
                    : $"{(diff / PreviousMtd) * 100:#,##0.00}%";

                return result;
            }
        }

        public double Margin { get; set; }

        public double PreviousMargin { get; set; }

        public string DiffMargin
        {
            get
            {
                var diff = PreviousMargin == 0
                    ? 0
                    : (Margin - PreviousMargin) / PreviousMargin;

                var result = diff > 0
                    ? $"+{diff * 100:#,##0.00}%"
                    : $"{diff * 100:#,##0.00}%";

                return result;
            }
        }
    }
}