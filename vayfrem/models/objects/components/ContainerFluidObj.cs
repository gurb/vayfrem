using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vayfrem.models.objects.components
{
    public class ContainerFluidObj : CanvasObj
    {
        public List<string> Rows { get; set; } = new List<string>();

        public ContainerFluidObj()
        {
            InitializeObject();
        }

        public override void InitializeObject()
        {
            this.ObjectType = enums.ObjectType.Canvas;
            this.Role = enums.CanvasRole.ContainerFluid;

            SetStyle();
        }

        public override void SetStyle()
        {
            Height = 100;
            Width = 400;
            BorderColor = new dtos.ColorDTO(255, 0, 0, 0);
            BorderThickness = 3;
            BorderRadius = 5;
        }
    }
}
