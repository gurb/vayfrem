using Avalonia.Media.Imaging;
using Avalonia;
using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Avalonia.Platform;
using System.Runtime.InteropServices;
using draftio.models.dtos;
using Avalonia.Media;

namespace draftio.services
{
    public class ColorPickerManager
    {
        uint width = 200;
        uint height = 200;

        Dictionary<string, RenderTexture> renderTextures = new Dictionary<string, RenderTexture>();

        RenderTexture colorBarRt;
        WriteableBitmap colorBarBitmap;
        public Avalonia.Controls.Image ColorBar { get; set; }

        public byte[] colorBarPixels { get; set; }

        LinearGradientBrush colorBarBrush;

        Vertex[] vertices = new Vertex[]
        {
            new Vertex(new Vector2f(0, 0), SFML.Graphics.Color.Red),
            new Vertex(new Vector2f(25, 0), SFML.Graphics.Color.Red),

            new Vertex(new Vector2f(0, 30), SFML.Graphics.Color.Yellow),
            new Vertex(new Vector2f(25, 30), SFML.Graphics.Color.Yellow),

            new Vertex(new Vector2f(0, 60), SFML.Graphics.Color.Green),
            new Vertex(new Vector2f(25, 60), SFML.Graphics.Color.Green),

            new Vertex(new Vector2f(0, 90), SFML.Graphics.Color.Cyan),
            new Vertex(new Vector2f(25, 90), SFML.Graphics.Color.Cyan),

            new Vertex(new Vector2f(0, 120), SFML.Graphics.Color.Blue),
            new Vertex(new Vector2f(25, 120), SFML.Graphics.Color.Blue),

            new Vertex(new Vector2f(0, 160), SFML.Graphics.Color.Magenta),
            new Vertex(new Vector2f(25, 160), SFML.Graphics.Color.Magenta),

            new Vertex(new Vector2f(0, 200), SFML.Graphics.Color.Red),
            new Vertex(new Vector2f(25, 200), SFML.Graphics.Color.Red)
        };

        Vertex[] verticesPalette = new Vertex[]
        {
            new Vertex(new Vector2f(0, 0), SFML.Graphics.Color.Blue),
            new Vertex(new Vector2f(200, 0), SFML.Graphics.Color.Red),
            new Vertex(new Vector2f(200, 200), SFML.Graphics.Color.Green),
            new Vertex(new Vector2f(0, 200), SFML.Graphics.Color.Yellow)
        };

        public ColorPickerManager() 
        {
            SetColorBar();
        }

        private void SetColorBar()
        {
            ColorBar = new Avalonia.Controls.Image();

            colorBarRt = new RenderTexture(25, 200);
            colorBarRt.Clear(SFML.Graphics.Color.Yellow);
            colorBarRt.Draw(vertices, 0, (uint)vertices.Length, PrimitiveType.TriangleStrip);
            colorBarRt.Display();

            byte[] pixels = colorBarRt.Texture.CopyToImage().Pixels;
            colorBarPixels = pixels;
            colorBarBitmap = new WriteableBitmap(new PixelSize(25, 200), new Vector(96, 96), PixelFormat.Rgba8888);
            using (var buffer = colorBarBitmap.Lock())
            {
                // Copy pixel data from SFML to Avalonia
                Marshal.Copy(pixels, 0, buffer.Address, pixels.Length);
            }

            ColorBar.Source = colorBarBitmap;
        }

        public RenderTexture GetTexture(string key)
        {
            if(!renderTextures.ContainsKey(key))
            {
                RenderTexture rt = new RenderTexture(width, height);
                rt.Clear(SFML.Graphics.Color.White);
                rt.Display();

                renderTextures.Add(key, rt);
            }

            return renderTextures[key];
        }


        public RenderTexture UpdateTexture(string key, Vertex[] vertices)
        {
            if (!renderTextures.ContainsKey(key))
            {
                RenderTexture rt = new RenderTexture(width, height);
                rt.Clear(SFML.Graphics.Color.White);
                rt.Display();

                renderTextures.Add(key, rt);
            }


            renderTextures[key].Clear(SFML.Graphics.Color.White);
            renderTextures[key].Draw(vertices, PrimitiveType.Quads);
            renderTextures[key].Display();

            return renderTextures[key];

        }


    }
}
