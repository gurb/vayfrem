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
    public class ImageObj : GObject
    {
        public bool IsEditMode { get; set; }

        public string? Path { get; set; }

        public enums.TextAlignment TextAlignment { get; set; } = enums.TextAlignment.MiddleCenter;

        public ImageObj()
        {
            InitializeObject();
        }

        public override void InitializeObject()
        {
            this.ObjectType = enums.ObjectType.Image;

            SetStyle();
        }

        public override void SetStyle()
        {
            base.SetStyle();

            Width = 250;
            Height = 250;

            BorderColor = new dtos.ColorDTO(0, 0, 0, 0);
            BorderThickness = 0;
            BorderRadius = 0;
        }
    }
}
