using Avalonia.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace draftio.models.dtos
{
    public class RectToolDTO
    {
        public ColorDTO Background { get; set; } = new ColorDTO(255, 0, 0, 0);
        public double Opacity { get; set; }
        public double BorderThickness { get; set; } = 1;
        public ColorDTO BorderColor { get; set; } = new ColorDTO(255, 0, 0, 0);
        public int BorderRadius { get; set; } = 0;
    }
}
