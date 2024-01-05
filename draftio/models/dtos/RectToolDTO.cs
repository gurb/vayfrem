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
        public Color Background { get; set; } = new Color(255, 255, 255, 255);
        public double Opacity { get; set; }
        public double BorderThickness { get; set; } = 1;
        public Color BorderColor { get; set; } = new Color(255, 0, 0, 0);
        public int BorderRadius { get; set; } = 0;
    }
}
