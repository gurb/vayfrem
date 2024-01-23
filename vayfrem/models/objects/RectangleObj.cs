using vayfrem.models.objects.@base;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vayfrem.models.objects
{
    public class RectangleObj: GObject
    {

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
