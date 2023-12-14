using draftio.models.enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace draftio.models
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
