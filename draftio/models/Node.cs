using Avalonia.Controls;
using draftio.models.enums;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace draftio.models
{
    [JsonDerivedType(typeof(File), typeDiscriminator: "FileType")]
    [JsonDerivedType(typeof(Folder), typeDiscriminator: "FolderType")]
    public class Node
    {
        public string? Name { get; set; }
        public NodeType Type { get; set; }
        public string? Guid { get; set; }

        public string? ParentGuid { get; set; }

        [JsonIgnore]
        public Node? ParentNode { get; set; }

        [JsonIgnore]
        public ObservableCollection<Node> Children { get; set; } = new ObservableCollection<Node>();

        public bool IsSelected { get; set; } = false;

        [JsonIgnore]
        public Control? ConnectedMenuControl { get; set; }

        [JsonIgnore]
        public Control? ConnectedTabControl { get; set; }

        [JsonIgnore]
        public Control? CaretControl { get; set; }

        [JsonIgnore]
        public Control? CloseControl { get; set; }

        public bool IsVisible { get; set; } = true;
        public bool IsDrew { get; set; } = false;
        public bool IsSaved { get; set; } = false;
    }
}
