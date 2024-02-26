using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vayfrem.models.objects.components
{
    public class RowObj : CanvasObj
    {
        public RowObj()
        {
            InitializeObject();
        }

        public override void InitializeObject()
        {
            this.ObjectType = enums.ObjectType.Canvas;
            this.Role = enums.CanvasRole.Row;

            SetStyle();
        }

        public override void SetStyle()
        {

        }
    }
}
