using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using vayfrem.models.objects.@base;
using vayfrem.models.structs;

namespace vayfrem.models.objects
{
    public class QuadraticBCObj: GObject
    {
        public Vector2 StartPoint { get; set; } = new Vector2(0, 0);
        public Vector2 Point1 { get; set; } = new Vector2(0,0);
        public Vector2 Point2 { get; set; } = new Vector2(0, 0);

        public QuadraticBCObj()
        {
            InitializeObject();
        }

        public override void InitializeObject()
        {
            this.ObjectType = enums.ObjectType.QuadraticBC;
        }
    }
}
