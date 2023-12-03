using draftio.models.objects.@base;
using draftio.models.structs;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace draftio.models.objects
{
    public class CanvasObj : Object
    {
        public List<IObject> Children { get; set; } = new List<IObject>();

        public CanvasObj() 
        {
            InitializeObject();
        }

        public void Add(IObject obj)
        {
            Children.Add(obj);
        }

        public void Remove(IObject obj)
        {
            Children.Remove(obj);
        }

        public override void InitializeObject()
        {
            this.ObjectType = enums.ObjectType.Canvas;
        }
    }
}
