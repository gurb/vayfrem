using Avalonia.Media;
using draftio.models.structs;
using System;
using System.Collections.Generic;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace draftio.models.lists
{
    public static class ListStorage
    {

        public static List<FontFamily> FontFamilies = new InstalledFontCollection().Families.Select(x => new FontFamily(x.Name))
                .OrderBy(x => x.Name).ToList();
       
        public static List<int> FontSizes = new List<int>
        {
            5,
            6,
            7,
            8,
            9,
            10,
            11,
            12,
            14,
            16,
            18,
            20,
            22,
            24,
            26,
            28,
            36,
            48,
            72
        };

        public static List<Dimension> Dimensions = new List<Dimension>
        {
            new Dimension
            {
                Name = "Custom",
                Width = 1920,
                Height = 1080,
                Type = DimensionType.Custom
            },
            new Dimension
            {
                Name = "iPhone SE",
                Width = 375,
                Height = 667,
                Type = DimensionType.iPhoneSE
            },
            new Dimension
            {
                Name = "iPad Mini",
                Width = 768,
                Height = 1024,
                Type = DimensionType.iPadMini
            },
            new Dimension
            {
                Name = "Samsung Galaxy S8+",
                Width = 360,
                Height = 740,
                Type = DimensionType.SamsungGalaxyS8Plus
            },
        };

    }
}
