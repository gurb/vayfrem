using vayfrem.models.structs;

namespace vayfrem.models.dtos
{
    public class ColorPickerDTO
    {
        public ColorDTO Color { get; set; } = new ColorDTO(255, 0, 0, 0);
        public ColorDTO BarColor { get; set; } = new ColorDTO(255, 0, 0, 0);
        public Vector2 ColorSelectPosition { get; set; } = new Vector2(0, 0);
        public Vector2 BarPosition { get; set; } = new Vector2(0, 0);
    }
}
