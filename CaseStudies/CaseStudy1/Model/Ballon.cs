using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaseStudy1.Model
{
    public sealed class Ballon
    {
        public double Length { get; set; }
        public double Angle { get; set; }
        public double Radius { get; set; }

        public int ColorIndex { get; set; }
        public string LayerName { get; set; }
        public string StringInput { get; set; }
        public Ballon(){}

    }
}
