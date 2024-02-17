using Avalonia.Media;
using vayfrem.models.objects.@base;
using vayfrem.models.structs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using vayfrem.models.enums;

namespace vayfrem.models.objects
{
    public class CanvasObj : GObject
    {
        public List<GObject> Children { get; set; } = new List<GObject>();

        public CanvasRole Role { get; set; }

        public CanvasObj() 
        {
            InitializeObject();
        }

        public void Add(GObject obj)
        {
            Children.Add(obj);
        }

        public void Remove(GObject obj)
        {
            Children.Remove(obj);
        }

        public override void InitializeObject()
        {
            this.ObjectType = enums.ObjectType.Canvas;
            this.Role = CanvasRole.None;

            base.InitializeObject();
            //Properties = new List<Property> 
            //{ 
            //    new Property(Value, Brushes.Transparent),
            //    new Property("Opacity", 1),
            //    new Property("BorderColor", Brushes.Black),
            //    new Property("BorderThickness", 1),
            //    new Property("BorderRadius", 0),
            //};
        }

        public override CanvasObj Copy()
        {
            CanvasObj obj = new CanvasObj();

            obj.X = this.X;
            obj.Y = this.Y;
            obj.Width = this.Width;
            obj.Height = this.Height;
            
            obj.Guid = System.Guid.NewGuid().ToString();
            obj.BackgroundColor = this.BackgroundColor;
            obj.BorderColor = this.BorderColor;
            obj.BorderRadius = this.BorderRadius;
            obj.BorderThickness = this.BorderThickness;
            obj.Opacity = this.Opacity;
            obj.ObjectType = this.ObjectType;
            obj.ZIndex = this.ZIndex;


            return obj;
        }
    }
}
