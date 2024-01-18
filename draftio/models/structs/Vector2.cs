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

        public static bool operator == (Vector2 v1, Vector2 v2)
        {
            if (v1.X == v2.X && v1.Y == v2.Y)
            {
                return true;
            }
            return false;
        }

        public static bool operator != (Vector2 v1, Vector2 v2)
        {
            if (v1.X == v2.X && v1.Y == v2.Y)
            {
                return false;
            }
            return true;
        }
    }
}
