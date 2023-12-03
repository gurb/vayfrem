using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace draftio.models.structs
{
    public class Vector2
    {
        public double X { get; set; } 
        public double Y { get; set; }

        public Vector2 (double x, double y) {  X = x; Y = y; }
    }
}
