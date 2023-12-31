using draftio.models.objects.@base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace draftio.models.objects
{
    public class SelectionObj : GObject
    {
        public GObject? SelectedObject { get; set; }

        public SelectionObj()
        {
            ZIndex = 999;
            InitializeObject();
        }

        public override void InitializeObject()
        {
            this.ObjectType = enums.ObjectType.Selection;
        }
    }
}
