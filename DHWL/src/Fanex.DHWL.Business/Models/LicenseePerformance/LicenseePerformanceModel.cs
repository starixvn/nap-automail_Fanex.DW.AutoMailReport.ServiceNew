using Fanex.DHWL.Business.Services;

namespace Fanex.DHWL.Business.Models.LicenseeDHWLLetstalk
{
    public class LicenseePerformanceModel
    {
        private readonly FormatService format = new FormatServiceImp();

        public string ProductShortName { get; set; }

        public string StrTO
        {
            get
            {
                return format.CutNumberOff(TO);
            }
        }

        public string StrWL
        {
            get
            {
                if (WL < 0)
                {
                    return $"({format.CutNumberOff(-WL)})";
                }

                return format.CutNumberOff(WL);
            }
        }

        public string StrMargin
        {
            get
            {
                if (Margin < 0)
                {
                    return $"({format.CutNumberOff(-Margin * 100)}%)";
                }

                return $"{format.CutNumberOff(Margin * 100)}%";
            }
        }

        public string StrTOMTD
        {
            get
            {
                return format.CutNumberOff(TOMTD);
            }
        }

        public string StrWLMTD
        {
            get
            {
                if (WLMTD < 0)
                {
                    return $"({format.CutNumberOff(-WLMTD)})";
                }

                return format.CutNumberOff(WLMTD);
            }
        }

        public string StrMarginMTD
        {
            get
            {
                if (MarginMTD < 0)
                {
                    return $"({format.CutNumberOff(-MarginMTD * 100)}%)";
                }

                return $"{format.CutNumberOff(MarginMTD * 100)}%";
            }
        }

        public string StrLicenseeFee
        {
            get
            {
                if (LicenseeFee < 0)
                {
                    return $"({format.CutNumberOff(-LicenseeFee)})";
                }

                return format.CutNumberOff(LicenseeFee);
            }
        }

        public string BreakColumn { get; } = string.Empty;

        public decimal TO { get; set; }

        public decimal WL { get; set; }

        public decimal Margin
        {
            get
            {
                return TO == 0 ? 0 : WL / TO;
            }
        }

        public decimal TOMTD { get; set; }

        public decimal WLMTD { get; set; }

        public decimal MarginMTD
        {
            get
            {
                return TOMTD == 0 ? 0 : WLMTD / TOMTD;
            }
        }

        public decimal LicenseeFee { get; set; }

        public int OrderByProduct { get; set; }

        public string ProductGroup { get; set; }
    }
}