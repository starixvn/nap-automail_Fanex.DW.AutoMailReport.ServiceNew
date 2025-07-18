using System;
using System.Globalization;

namespace Fanex.DHWL.Business.Services
{
    public interface FormatService
    {
        string CutNumberOff(decimal value);
    }

    public class FormatServiceImp : FormatService
    {
        public string CutNumberOff(decimal value)
        {
            // Handle zero and very small values
            if (Math.Abs(value) < 0.001m)
            {
                return "0";
            }

            // For values < 1 Thousand: apply standard ROUND and format
            if (Math.Abs(value) < 1000m)
            {
                var rounded = Math.Round(value, 0);
                if (Math.Abs(value - rounded) < 0.001m)
                {
                    // If the difference is very small, treat as integer
                    return ((int)rounded).ToString();
                }
                
                // For decimal values, round to 2 decimal places and remove trailing zeros
                var decimalRounded = Math.Round(value, 2);
                var result = decimalRounded.ToString("F2", CultureInfo.InvariantCulture);
                return result.TrimEnd('0').TrimEnd('.');
            }

            // For values >= 1 Thousand and < 1 Million: divide by 1000, ROUND, format with "k"
            if (Math.Abs(value) < 1000000m)
            {
                var divided = value / 1000m;
                var rounded = Math.Round(divided, 1);
                return rounded.ToString("F1", CultureInfo.InvariantCulture) + "k";
            }

            // For values >= 1 Million and < 1 Billion: divide by 1000000, ROUND, format with "m"
            if (Math.Abs(value) < 1000000000m)
            {
                var divided = value / 1000000m;
                var rounded = Math.Round(divided, 2);
                return rounded.ToString("F2", CultureInfo.InvariantCulture) + "m";
            }

            // For values >= 1 Billion: divide by 1000000000, ROUND, format with "b"
            var billionDivided = value / 1000000000m;
            var billionRounded = Math.Round(billionDivided, 2);
            return billionRounded.ToString("F2", CultureInfo.InvariantCulture) + "b";
        }
    }
}