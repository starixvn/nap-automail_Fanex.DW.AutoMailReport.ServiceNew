using System;
using System.Globalization;

namespace Fanex.MarginChart.Business.Services
{
    public interface FormatService
    {
        string CutNumberOff(decimal value);

        double Truncate(double value, int precision);
    }

    public class FormatServiceImp : FormatService
    {
        public string CutNumberOff(decimal value)
        {
            if (Math.Abs(value) < 1000)
            {
                return value.ToString("#0", CultureInfo.InvariantCulture);
            }

            if (Math.Abs(value) < 1000000)
            {
                return value.ToString("#,k", CultureInfo.InvariantCulture);
            }

            if (Math.Abs(value) < 1000000000)
            {
                return value.ToString("#,,.#0m", CultureInfo.InvariantCulture);
            }

            return value.ToString("#,,,.#0b", CultureInfo.InvariantCulture);
        }

        public double Truncate(double value, int precision)
        {
            double step = Math.Pow(10, precision);
            var tmp = Math.Truncate(step * value);
            return tmp / step;
        }
    }
}