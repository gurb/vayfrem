﻿using draftio.models.enums;
using draftio.models.structs;
using System.Text.Json.Serialization;

namespace draftio.models.objects
{
    public interface IObject
    {
        string Guid { get; set; }
        string? ParentGuid { get; set; }

        double X { get; set; }
        double Y { get; set; }
        double Width { get; set; }
        double Height { get; set; }
        Avalonia.Media.Color BackgroundColor { get; set; }
        Avalonia.Media.Color BorderColor { get; set; }
        double BorderThickness { get; set; }

        [JsonIgnore]
        IObject? Parent { get; set; }
        ObjectType ObjectType { get; set; }
        double WorldX { get; }
        double WorldY { get; }
    }
}
