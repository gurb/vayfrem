using vayfrem.models.enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vayfrem.models
{
    public class Folder: Node
    {
        public Folder? ParentFolder { get; set; }
        
        public Folder ()
        {
            Type = NodeType.Folder;
        }
    }
}
