using draftio.models.enums;
using draftio.models.objects;
using draftio.models.objects.@base;
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
        public string? Data { get; set; }
        public string? TabGuid { get; set; }
        public List<GObject> Objects { get; set; } = new List<GObject>();

        public File ()
        {
            Type = NodeType.File;
        }

        public File (string name)
        {
            Name = name;
            Type = NodeType.File;
        }
    }
}
