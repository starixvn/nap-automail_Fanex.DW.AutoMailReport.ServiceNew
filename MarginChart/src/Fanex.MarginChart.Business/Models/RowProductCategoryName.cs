using System.Collections.Generic;

namespace Fanex.MarginChart.Business.Models
{
    public class RowProductCategoryName
    {
        public const string Soccer = "Soccer";
        public const string OtherSport = "OtherSport";
        public const string NonSport = "NonSport";

        public static List<string> ProductCategories = new List<string>
        {
            Soccer,
            OtherSport,
            NonSport
        };
    }
}