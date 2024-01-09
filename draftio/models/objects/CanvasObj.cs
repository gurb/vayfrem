using Avalonia.Media;
using draftio.models.objects.@base;
using draftio.models.structs;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace draftio.models.objects
{
    public class CanvasObj : GObject
    {
        public List<GObject> Children { get; set; } = new List<GObject>();

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

            //Properties = new List<Property> 
            //{ 
            //    new Property(Value, Brushes.Transparent),
            //    new Property("Opacity", 1),
            //    new Property("BorderColor", Brushes.Black),
            //    new Property("BorderThickness", 1),
            //    new Property("BorderRadius", 0),
            //};
        }
    }
}
