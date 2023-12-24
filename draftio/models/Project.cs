using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace draftio.models
{

    public class Project
    {
        public string? ProjectName { get; set; }
        public Folder? RootFolder { get; set; }
        public ObservableCollection<Node> Nodes { get; set; } = new();
    }
}
