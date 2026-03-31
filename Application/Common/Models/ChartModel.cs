using System.Collections.Generic;

namespace Biobrain.Application.Common.Models
{
    public class ChartModel
    {
        public string ChartImagePath { get; set; }
        public List<ChartLegendModel> Legend { get; set; }
    }
}