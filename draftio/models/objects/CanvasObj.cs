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
        }
    }
}
