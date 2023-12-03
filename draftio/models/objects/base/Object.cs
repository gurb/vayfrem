using draftio.models.enums;
using draftio.models.structs;

namespace draftio.models.objects.@base
{
    public class Object : IObject
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public ColorS BackgroundColor { get; set; } = new ColorS(255, 255, 255, 255);
        public ColorS BorderColor { get; set; } = new ColorS(0, 0, 0, 255);
        public float BorderSize { get; set; }
        public ObjectType ObjectType { get; set; }

        public virtual void InitializeObject () { }
    }
}
