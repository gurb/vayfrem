using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace draftio.models
{
    public class Folder
    {
        public string? Name { get; set; }
        public Folder? ParentFolder { get; set; }
    }
}
