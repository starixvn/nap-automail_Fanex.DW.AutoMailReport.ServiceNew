using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fanex.DHWL.Business.Models
{
    public class ChartAreasPosition
    {
        public Position Position { get; set; } = new Position();

        public InnerPlotPosition InnerPlotPosition { get; set; } = new InnerPlotPosition();
    }
}