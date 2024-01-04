using Avalonia.Controls;
using draftio.models.enums;
using draftio.models.structs;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace draftio.models.objects.@base
{
    [JsonDerivedType(typeof(CanvasObj), typeDiscriminator: "CanvasObjType")]
    [JsonDerivedType(typeof(TextObj), typeDiscriminator: "TextObjType")]
    public class GObject : IObject
    {
        public string? Guid { get; set; }
        public string? ParentGuid { get; set; }

        public double X { get; set; }
        public double Y { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public int ZIndex { get; set; } = 1;
        public ColorS BackgroundColor { get; set; } = new ColorS(255, 255, 255, 255);
        public ColorS BorderColor { get; set; } = new ColorS(0, 0, 0, 255);
        public float BorderSize { get; set; }
        public ObjectType ObjectType { get; set; }

        [JsonIgnore]
        public bool IsDrew { get; set; }

        [JsonIgnore]
        public bool IsVisible { get; set; } = true;

        [JsonIgnore]
        public bool IsSelected { get; set; }

        [JsonIgnore]
        public IObject? Parent { get; set; }

        [JsonIgnore]
        public Control? ConnectedMenuControl { get; set; }

        [JsonIgnore]
        public Control? ConnectedTabControl { get; set; }

        [JsonIgnore]
        public Control? CaretControl { get; set; }

        [JsonIgnore]
        public Control? CloseControl { get; set; }

        public List<Property> Properties { get; set; } = new List<Property>();

        public double WorldX
        {
            get
            {
                if (Parent == null)
                    return X;
                else
                    return X + Parent.WorldX;
            }
        }
        public double WorldY
        {
            get
            {
                if (Parent == null)
                    return Y;
                else
                    return Y + Parent.WorldY;
            }
        }

        public virtual void InitializeObject () 
        {
        }
    }
}
