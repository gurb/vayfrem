﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vayfrem.models.objects.components
{
    public class ContainerObj : CanvasObj
    {
        public List<string> Rows { get; set; } = new List<string>();

        public ContainerObj()
        {
            InitializeObject();
        }

        public override void InitializeObject()
        {
            this.ObjectType = enums.ObjectType.Canvas;
            this.Role = enums.CanvasRole.Container;

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
