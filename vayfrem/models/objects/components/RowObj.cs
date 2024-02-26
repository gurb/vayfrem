using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vayfrem.models.objects.components
{
    public class RowObj : CanvasObj
    {
        public List<string> Columns = new List<string>();

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
            Height = 50;

            BorderColor = new dtos.ColorDTO(255, 0, 0, 0);
            BorderThickness = 3;
            BorderRadius = 5;
        }
    }
}
