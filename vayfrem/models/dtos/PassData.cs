using vayfrem.models.enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using vayfrem.models.structs;

namespace vayfrem.models.dtos
{
    public class PassData
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public ObjectType SelectedObjectType { get; set; }

        // Quadratic Bezier Curve
        public Vector2? StartPoint { get; set; }
        public Vector2? Point1 { get; set; }
        public Vector2? Point2 { get; set; }
    }
}
