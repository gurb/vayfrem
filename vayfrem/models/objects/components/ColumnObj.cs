using Avalonia.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using vayfrem.models.dtos;
using vayfrem.models.objects.@base;

namespace vayfrem.models.objects.components
{
    public class ColumnObj : CanvasObj
    {

        public ColumnObj()
        {
            InitializeObject();
        }

        public override void InitializeObject()
        {
            this.ObjectType = enums.ObjectType.Canvas;
            this.Role = enums.CanvasRole.Column;

            SetStyle();
        }

        public override void SetStyle()
        {

        }
    }
}
