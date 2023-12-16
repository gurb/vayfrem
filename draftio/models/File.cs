using draftio.models.enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace draftio.models
{
    public class File: Node
    {
        public FileType FileType { get; set; }
        public Folder? ParentFolder { get; set; }
        public string? Data { get; set; }
        public string? TabGuid { get; set; }

        public File ()
        {
            Type = NodeType.File;
        }
    }
}
