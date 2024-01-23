using vayfrem.models.dtos;
using vayfrem.models.enums;
using vayfrem.models.structs;
using System.Text.Json.Serialization;

namespace vayfrem.models.objects
{
    public interface IObject
    {
        string Guid { get; set; }
        string? ParentGuid { get; set; }

        double X { get; set; }
        double Y { get; set; }
        double Width { get; set; }
        double Height { get; set; }
        ColorDTO BackgroundColor { get; set; }
        ColorDTO BorderColor { get; set; }
        double BorderThickness { get; set; }

        [JsonIgnore]
        IObject? Parent { get; set; }
        ObjectType ObjectType { get; set; }
        double WorldX { get; }
        double WorldY { get; }
    }
}
