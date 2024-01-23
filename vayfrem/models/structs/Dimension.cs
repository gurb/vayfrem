using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vayfrem.models.structs
{

    public enum DimensionType
    {
        Custom,   // 1920X1080
        iPhoneSE, // 375X667
        iPadMini, // 768X1024
        SamsungGalaxyS8Plus, // 360x740
    }

    public class Dimension
    {
        public string? Name { get; set; }
        public DimensionType Type { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

    }
}
