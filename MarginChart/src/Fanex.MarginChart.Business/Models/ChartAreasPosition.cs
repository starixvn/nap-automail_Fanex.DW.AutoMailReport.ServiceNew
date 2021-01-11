namespace Fanex.MarginChart.Business.Models
{
    public class ChartAreasPosition
    {
        public Position Position { get; set; } = new Position();

        public InnerPlotPosition InnerPlotPosition { get; set; } = new InnerPlotPosition();
    }
}