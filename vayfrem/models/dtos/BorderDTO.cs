using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vayfrem.models.dtos
{
    public class BorderDTO
    {
        public double Thickness { get; set; }
        public double LeftThickness { get; set; }
        public double TopThickness { get; set; }
        public double RightThickness { get; set; }
        public double BottomThickness { get; set; }

        public bool Relative { get; set; } = false;

        public string GetRelative()
        {
            // L - T - R - B
            return $"{LeftThickness} {TopThickness} {RightThickness} {BottomThickness}";
        }

    }
}
