﻿using Avalonia.Media;
using PdfSharpCore.Drawing;
using System.Linq;
using System.Text.Json.Serialization;

namespace vayfrem.models.dtos
{
    public class ColorDTO
    {
        public byte A { get; set; }
        public byte R { get; set; }
        public byte G { get; set; }
        public byte B { get; set; }

        [JsonConstructor]
        public ColorDTO(byte A, byte R, byte G, byte B) 
        {
            this.A = A;
            this.R = R;
            this.G = G;
            this.B = B;
        }

        public ColorDTO(Color color)
        {
            this.A = color.A;
            this.R = color.R;
            this.G = color.G;
            this.B = color.B;
        }

        public Color ToColor()
        {
            return Color.FromArgb(A, R, G, B);
        }

        public string ToHex()
        {
            byte[] byteArray = { R, G, B };
            string hex = string.Join("", byteArray.Select(x => x.ToString("X2")));
            return hex;
        }

        public ColorDTO toCopy()
        {
            return new ColorDTO(A, R, G, B);
        }

        public XColor ToXColor()
        {
            return XColor.FromArgb(A, R, G, B);
        }

        public string HTMLColor(byte opacity = 255)
        {
            return $"rgba({R},{G},{B},{opacity})";
        }

        public XColor ToXColor(int opacity)
        {
            return XColor.FromArgb((byte)(opacity), R, G, B);
        }
    }
}
