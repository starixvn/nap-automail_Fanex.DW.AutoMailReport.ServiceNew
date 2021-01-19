namespace Fanex.MarginChart.Business.Models
{
    public class ProductGroup
    {
        public string LetstalkGroup { get; set; } = string.Empty;

        public string ProductCategory { get; set; }

        public string Subject { get; set; } = string.Empty;

        public string HeaderChart { get; set; } = string.Empty;

        public int AxisXIntervalTurnover { get; set; }

        public double AxisY2IntervalMargin { get; set; }

        public int ConvertValue { get; set; }

        public bool IsChartAttachment { get; set; }
    }
}