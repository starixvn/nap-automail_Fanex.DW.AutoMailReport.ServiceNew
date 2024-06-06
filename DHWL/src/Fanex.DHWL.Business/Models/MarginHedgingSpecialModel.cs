using Fanex.DHWL.Business.Services;

namespace Fanex.DHWL.Business.Models
{
    public class MarginHedgingSpecialModel
    {
        private readonly FormatService format = new FormatServiceImp();

        public string ProductShortName { get; set; }

        public string StrSeven89YTO => format.CutNumberOff(Seven89YTO);

        public string StrSeven89YWL
        {
            get
            {
                if (Seven89YWL < 0)
                {
                    return $"({format.CutNumberOff(-Seven89YWL)})";
                }

                return format.CutNumberOff(Seven89YWL);
            }
        }

        public decimal Seven89YMargin
        {
            get
            {
                if (Seven89YTO == 0)
                {
                    return 0;
                }

                return Seven89YWL / Seven89YTO;
            }
        }

        public string StrHedgingTO => format.CutNumberOff(HedgingTO);

        public string StrHedgingWL
        {
            get
            {
                if (HedgingWL < 0)
                {
                    return $"({format.CutNumberOff(-HedgingWL)})";
                }

                return format.CutNumberOff(HedgingWL);
            }
        }

        public decimal HedgingMargin
        {
            get
            {
                if (HedgingTO == 0)
                {
                    return 0;
                }

                return HedgingWL / HedgingTO;
            }
        }

        public string StrSpecialTO => format.CutNumberOff(SpecialTO);

        public string StrSpecialWL
        {
            get
            {
                if (SpecialWL < 0)
                {
                    return $"({format.CutNumberOff(-SpecialWL)})";
                }

                return format.CutNumberOff(SpecialWL);
            }
        }

        public decimal SpecialMargin
        {
            get
            {
                if (SpecialTO == 0)
                {
                    return 0;
                }

                return SpecialWL / SpecialTO;
            }
        }

        public string StrMBCTO => format.CutNumberOff(MBCTO);

        public string StrMBCWL
        {
            get
            {
                if (MBCWL < 0)
                {
                    return $"({format.CutNumberOff(-MBCWL)})";
                }

                return format.CutNumberOff(MBCWL);
            }
        }

        public decimal MBCMargin
        {
            get
            {
                if (MBCTO == 0)
                {
                    return 0;
                }

                return MBCWL / MBCTO;
            }
        }

        public string StrLicenseeTO => format.CutNumberOff(LicenseeTO);

        public string StrLicenseeWL
        {
            get
            {
                if (LicenseeWL < 0)
                {
                    return $"({format.CutNumberOff(-LicenseeWL)})";
                }

                return format.CutNumberOff(LicenseeWL);
            }
        }

        public decimal LicenseeMargin
        {
            get
            {
                if (LicenseeTO == 0)
                {
                    return 0;
                }

                return LicenseeWL / LicenseeTO;
            }
        }

        public string StrTotalTO => format.CutNumberOff(TotalTO);

        public string StrTotalWL
        {
            get
            {
                if (TotalWL < 0)
                {
                    return $"({format.CutNumberOff(-TotalWL)})";
                }

                return format.CutNumberOff(TotalWL);
            }
        }

        public decimal TotalMargin
        {
            get
            {
                if (TotalTO == 0)
                {
                    return 0;
                }

                return TotalWL / TotalTO;
            }
        }

        public string ProductShortName2 => ProductShortName;

        public string BreakColumn => string.Empty;

        public decimal Seven89YTO { get; set; }

        public decimal Seven89YWL { get; set; }

        public decimal HedgingTO { get; set; }

        public decimal HedgingWL { get; set; }

        public decimal SpecialTO { get; set; }

        public decimal SpecialWL { get; set; }

        public decimal MBCTO { get; set; }

        public decimal MBCWL { get; set; }

        public decimal LicenseeTO { get; set; }

        public decimal LicenseeWL { get; set; }

        public decimal TotalTO { get; set; }

        public decimal TotalWL { get; set; }

        public int OrderByProduct { get; set; }

        public string ProductGroup { get; set; }
    }
}