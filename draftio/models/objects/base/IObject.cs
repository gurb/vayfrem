using draftio.models.enums;
using draftio.models.structs;

namespace draftio.models.objects
{
    public interface IObject
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public ColorS BackgroundColor { get; set; }
        public ColorS BorderColor { get; set; }
        public float BorderSize { get; set; }
        public ObjectType ObjectType { get; set; }
    }
}
