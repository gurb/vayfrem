using draftio.models.objects.@base;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace draftio.models.objects
{
    public class RectangleObj: Object
    {
        public IObject? Parent { get; set; }

        public RectangleObj() 
        {
            InitializeObject();
        }

        public override void InitializeObject()
        {
            this.ObjectType = enums.ObjectType.Rectangle;
        }
    }
}
