using Avalonia.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vayfrem.models.dtos
{
    public class TextToolDTO
    {
        public int SelectedFontFamilyIndex { get; set; } = 0;
        public string? FontFamily { get; set; } = "Arial";
        public int SelectedFontSizeIndex { get; set; } = 8;
        public int SelectedFontWeightIndex { get; set; } = 6;
        public int FontSize { get; set; } = 14;
        public ColorDTO FontColor { get; set; } = new ColorDTO(255, 0, 0, 0);

        public enums.FontWeight FontWeight = enums.FontWeight.Regular;
    }
}
