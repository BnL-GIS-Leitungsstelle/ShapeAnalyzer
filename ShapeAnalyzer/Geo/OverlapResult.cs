using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShapeAnalyzer.Geo
{
    public class OverlapResult
    {      
        public double OverlapPercentage { get; set; }

        public double OverlapAreaInHa { get; set; }

        public double NonOverlappingAreaInHa { get; set; }
    }
}
