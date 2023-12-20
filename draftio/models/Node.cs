using Avalonia.Controls;
using draftio.models.enums;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace draftio.models
{
    public class Node
    {
        public string? Name { get; set; }
        public NodeType Type { get; set; }
        public Node? ParentNode { get; set; }
        public ObservableCollection<Node> Children { get; set; } = new ObservableCollection<Node>();
        public bool IsSelected { get; set; } = false;
        public Control? ConnectedControl { get; set; }
        public bool IsVisible { get; set; }
        public bool IsDrew { get; set; } = false;
    }
}
