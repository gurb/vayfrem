using draftio.models.enums;
using draftio.models.structs;

namespace draftio.models.objects
{
    public interface IObject
    {
        double X { get; set; }
        double Y { get; set; }
        double Width { get; set; }
        double Height { get; set; }
        ColorS BackgroundColor { get; set; }
        ColorS BorderColor { get; set; }
        float BorderSize { get; set; }
        IObject? Parent { get; set; }
        ObjectType ObjectType { get; set; }
        double WorldX { get; }
        double WorldY { get; }
    }
}
