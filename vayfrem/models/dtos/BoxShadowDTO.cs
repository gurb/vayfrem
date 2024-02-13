using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vayfrem.models.dtos
{
    public class BoxShadowDTO
    {
        public double HOffset { get; set; } = 0;
        public double VOffset { get; set; } = 0;
        public double Blur { get; set; } = 0;
        public double Spread { get; set; } = 0;
        public bool Inset { get; set; }

        public BoxShadowDTO() { }
    }
}
