using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace draftio.models.structs
{
    public class ColorS
    {
        public byte R { get; set; } = 255;
        public byte G { get; set; } = 255;
        public byte B { get; set; } = 255;
        public byte A { get; set; } = 255;

        public ColorS(byte r, byte g, byte b, byte a)
        {
            this.R = r;
            this.G = g;
            this.B = b;
            this.A = a;
        }

        static public ColorS Red { get { return new ColorS(255, 0, 0, 255); } }
        static public ColorS Green { get { return new ColorS(0, 255, 0, 255); } }
        static public ColorS Blue { get { return new ColorS(0, 0, 255, 255); } }
        static public ColorS Transparent { get { return new ColorS(255, 255, 255, 0); } }
        static public ColorS Black { get { return new ColorS(0, 0, 0, 255); } }
    }
}
