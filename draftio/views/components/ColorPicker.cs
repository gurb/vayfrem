using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using draftio.models.dtos;
using draftio.services;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using SkiaSharp;
using System;
using System.Drawing.Imaging;
using System.IO;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Xml.Linq;


namespace draftio.views.components
{
    public class ColorPicker: Canvas
    {
        public string Name { get; set; }

        ColorPickerManager colorPickerManager;

        Grid layout { get; set; }

        Flyout flyout = new Flyout();

        Border BorderColorArea = new Border();
        Border BorderColorBar = new Border();

        Canvas ColorArea = new Canvas();
        Canvas ColorBar = new Canvas();

        LinearGradientBrush paletteBrush;

        LinearGradientBrush colorBarBrush;

        SKRect rect;

        public delegate void ValueChangeDelegate();
        public ValueChangeDelegate? ValueChanged;


        Rectangle barSelector { get; set; } = new Rectangle();


        Avalonia.Controls.Image palette = new Avalonia.Controls.Image();
        Avalonia.Controls.Image colorBar = new Avalonia.Controls.Image();


        Vertex[] vertices = new Vertex[]
        {
            new Vertex(new Vector2f(0, 0), SFML.Graphics.Color.White),
            new Vertex(new Vector2f(200, 0), SFML.Graphics.Color.Red),
            new Vertex(new Vector2f(200, 200), SFML.Graphics.Color.Black),
            new Vertex(new Vector2f(0, 200), SFML.Graphics.Color.Black)
        };



        public ColorDTO SelectedColor { get; set; }
        public ColorDTO SelectedBarColor { get; set; }


        public ColorPicker(string name)
        {
            Name = name;

            layout = new Grid();
            layout.ColumnDefinitions = new ColumnDefinitions("200, 5, 25");

            colorPickerManager = App.GetService<ColorPickerManager>();

            Background = Avalonia.Media.Brushes.Red;
            HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch;
            Height = 26;

            this.PointerPressed += ColorPicker_PointerPressed;

            ColorArea.Width = 200;
            ColorArea.Height = 200;

            ColorBar.Width = 25;
            ColorBar.Height = 200;


            colorPickerManager.GetTexture(name).Draw(vertices, PrimitiveType.Quads);
            colorPickerManager.GetTexture(name).Display();

            setPaletteBrush();

            //ColorArea.Background = paletteBrush;
            ColorArea.PointerPressed += ColorArea_PointerPressed;
            colorBar.Source = colorPickerManager.ColorBar.Source;
            ColorBar.Children.Clear();
            ColorBar.Children.Add(colorBar);
            //ColorBar.Background = colorPickerManager.ColorBar;
            ColorBar.PointerPressed += ColorBar_PointerPressed;
            Canvas.SetTop(colorBar, 0);
            Canvas.SetLeft(colorBar, 0);

            Grid.SetColumn(BorderColorArea, 0);
            Grid.SetColumn(BorderColorBar, 2);

            BorderColorArea.Child = ColorArea;
            BorderColorBar.Child = ColorBar;

            layout.Children.Add(BorderColorArea);
            layout.Children.Add(BorderColorBar);

            setBarSelector();
            setBarBrush();

            flyout.Content = layout;

            this.ContextFlyout = flyout;

        }

        private void ColorBar_PointerPressed(object? sender, Avalonia.Input.PointerPressedEventArgs e)
        {
            Avalonia.Point point = e.GetPosition(sender as Control);

            int x = (int)point.X;
            int y = (int)point.Y;

            Avalonia.Media.Color pixelColor = GetPixelBarColor(x, y);

            SelectedBarColor = new ColorDTO(pixelColor);

            vertices[1] = new Vertex(new Vector2f(200, 0), new SFML.Graphics.Color(SelectedBarColor.R, SelectedBarColor.G, SelectedBarColor.B, 255));
            colorPickerManager.UpdateTexture(Name, vertices);

            Canvas.SetLeft(barSelector, 0);
            Canvas.SetTop(barSelector, y);


            setPaletteBrush();
            //ColorArea.Background = paletteBrush;


        }

        private void setBarSelector()
        {
            barSelector.Width = 25;
            barSelector.Height = 5;
            barSelector.Fill = Brushes.Black;

            ColorBar.Children.Add(barSelector);
            Canvas.SetLeft(barSelector, 0);
            Canvas.SetTop(barSelector, 0);
        }


        private void setPaletteBrush()
        {
            ColorArea.Children.Clear();

            //Avalonia.Media.Imaging.Bitmap avaloniaBitmap;

            byte[] pixels = colorPickerManager.GetTexture(Name).Texture.CopyToImage().Pixels;
            var avaloniaBitmap = new WriteableBitmap(new PixelSize(200, 200), new Vector(96, 96), Avalonia.Platform.PixelFormat.Rgba8888);


            using (var buffer = avaloniaBitmap.Lock())
            {
                // Copy pixel data from SFML to Avalonia
                Marshal.Copy(pixels, 0, buffer.Address, pixels.Length);
            }

            palette.Source = avaloniaBitmap;

            ColorArea.Children.Add(palette);

            Canvas.SetLeft(palette, 0);
            Canvas.SetTop(palette, 0);
            //paletteBrush = new LinearGradientBrush();

        }

        private void setBarBrush()
        {
            ColorBar.Children.Clear();

            //Avalonia.Media.Imaging.Bitmap avaloniaBitmap;

            byte[] pixels = colorPickerManager.colorBarPixels;
            var avaloniaBitmap = new WriteableBitmap(new PixelSize(25, 200), new Vector(96, 96), Avalonia.Platform.PixelFormat.Rgba8888);


            using (var buffer = avaloniaBitmap.Lock())
            {
                // Copy pixel data from SFML to Avalonia
                Marshal.Copy(pixels, 0, buffer.Address, pixels.Length);
            }

            colorBar.Source = avaloniaBitmap;

            ColorBar.Children.Add(colorBar);
            ColorBar.Children.Add(barSelector);

            Canvas.SetLeft(colorBar, 0);
            Canvas.SetTop(colorBar, 0);
            //paletteBrush = new LinearGradientBrush();

        }


        private void ColorPicker_PointerPressed(object? sender, Avalonia.Input.PointerPressedEventArgs e)
        {
            flyout.ShowAt(this);
        }

        private void ColorArea_PointerPressed(object? sender, Avalonia.Input.PointerPressedEventArgs e)
        {
            Avalonia.Point point = e.GetPosition(sender as Control);

            int x = (int)point.X;
            int y = (int)point.Y;

            Avalonia.Media.Color pixelColor = GetPixelColor(x, y);

            SelectedColor = new ColorDTO(pixelColor);

            if(ValueChanged != null)
            {
                ValueChanged.Invoke();
            }

            Background = new SolidColorBrush(pixelColor);

        }
        public static SKBitmap RenderTargetBitmapToSKBitmap(RenderTargetBitmap renderTargetBitmap)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                renderTargetBitmap.Save(stream);

                stream.Seek(0, SeekOrigin.Begin);

                return SKBitmap.Decode(stream);
            }
        }

        private Avalonia.Media.Color GetPixelColor(int x, int y)
        {
            var renderTarget = new RenderTargetBitmap(new PixelSize((int)ColorArea.Bounds.Width, (int)ColorArea.Bounds.Height), new Vector(96, 96));
            renderTarget.Render(ColorArea);

            SKBitmap skBitmap = RenderTargetBitmapToSKBitmap(renderTarget);

            SKColor color = skBitmap.GetPixel(x, y);

            return Avalonia.Media.Color.FromArgb(color.Alpha, color.Red, color.Green, color.Blue);
        }

        private Avalonia.Media.Color GetPixelBarColor(int x, int y)
        {
            var renderTarget = new RenderTargetBitmap(new PixelSize(25, 200), new Vector(96, 96));
            renderTarget.Render(ColorBar);

            SKBitmap skBitmap = RenderTargetBitmapToSKBitmap(renderTarget);

            SKColor color = skBitmap.GetPixel(x, y);

            return Avalonia.Media.Color.FromArgb(color.Alpha, color.Red, color.Green, color.Blue);
        }

    }
}
