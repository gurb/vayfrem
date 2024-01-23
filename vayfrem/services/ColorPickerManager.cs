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
using vayfrem.models.dtos;
using Avalonia.Media;
using System.Diagnostics;
using vayfrem.models.lists;

namespace vayfrem.services
{
    public class ColorPickerManager
    {
        uint width = 255;
        uint height = 255;

        Dictionary<string, RenderTexture> renderTextures = new Dictionary<string, RenderTexture>();

        RenderTexture colorBarRt;
        WriteableBitmap colorBarBitmap;
        public Avalonia.Controls.Image ColorBar { get; set; }

        public byte[] colorBarPixels { get; set; }

        LinearGradientBrush colorBarBrush;

        Sprite pickerForeground;
        Texture pickerTexture;


        Sprite hueSprite;
        Texture hueTexture;

        Vertex[] vertices = new Vertex[]
        {
            new Vertex(new Vector2f(0, 0), new SFML.Graphics.Color(255, 0, 0)),
            new Vertex(new Vector2f(25, 0), new SFML.Graphics.Color(255, 0, 0)),

            new Vertex(new Vector2f(0, 42.5f), new SFML.Graphics.Color(255, 255, 0)),
            new Vertex(new Vector2f(25, 42.5f), new SFML.Graphics.Color(255, 255, 0)),

            new Vertex(new Vector2f(0, 85f), new SFML.Graphics.Color(0, 255, 0)),
            new Vertex(new Vector2f(25, 85), new SFML.Graphics.Color(0, 255, 0)),

            new Vertex(new Vector2f(0, 127.5f), new SFML.Graphics.Color(0, 255, 255)),
            new Vertex(new Vector2f(25, 127.5f), new SFML.Graphics.Color(0, 255, 255)),

            new Vertex(new Vector2f(0, 170),  new SFML.Graphics.Color(0, 0, 255)),
            new Vertex(new Vector2f(25, 170), new SFML.Graphics.Color(0, 0, 255)),

            new Vertex(new Vector2f(0, 212.5f),  new SFML.Graphics.Color(255, 0, 255)),
            new Vertex(new Vector2f(25, 212.5f),  new SFML.Graphics.Color(255, 0, 255)),

            new Vertex(new Vector2f(0, 255), new SFML.Graphics.Color(255, 0, 0)),
            new Vertex(new Vector2f(25, 255), new SFML.Graphics.Color(255, 0, 0)),
        };

        Vertex[] verticesPalette = new Vertex[]
        {
            new Vertex(new Vector2f(0, 0), SFML.Graphics.Color.Red),
            new Vertex(new Vector2f(255, 0), SFML.Graphics.Color.Red),
            new Vertex(new Vector2f(255, 255), SFML.Graphics.Color.Red),
            new Vertex(new Vector2f(0, 255), SFML.Graphics.Color.Red)
        };

        public ColorPickerManager() 
        {
            LoadTexture();
            SetColorBar();
        }

        private void LoadTexture()
        {
            try
            {
                byte[] pixels = Convert.FromBase64String(ListStorage.MapHueBase64);
                Image image = new Image((uint)256, (uint)256, pixels);
                pickerTexture = new Texture(image);
                image.Dispose();
                pickerForeground = new Sprite(pickerTexture);
                //string base64 = Convert.ToBase64String(pickerTexture.CopyToImage().Pixels);

                pickerForeground.Position = new Vector2f(0, 0);
                hueTexture = new Texture("assets\\bar-hue.png");
                hueSprite = new Sprite(hueTexture);
                hueSprite.Position = new Vector2f(0, 0);
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message.ToString());
                
            }
            
        }

        private void SetColorBar()
        {
            ColorBar = new Avalonia.Controls.Image();

            colorBarRt = new RenderTexture(25, 255);
            colorBarRt.Clear(SFML.Graphics.Color.Yellow);
            colorBarRt.Draw(vertices, 0, (uint)vertices.Length, PrimitiveType.TriangleStrip);
            colorBarRt.Display();

            byte[] pixels = colorBarRt.Texture.CopyToImage().Pixels;
            colorBarPixels = pixels;
            colorBarBitmap = new WriteableBitmap(new PixelSize(25, 255), new Vector(96, 96), PixelFormat.Rgba8888);
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
            if(pickerForeground != null)
            {
                renderTextures[key].Draw(pickerForeground);
            }
            renderTextures[key].Display();

            return renderTextures[key];
        }
    }
}
