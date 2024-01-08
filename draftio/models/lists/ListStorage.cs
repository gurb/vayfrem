using Avalonia.Media;
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

    }
}
