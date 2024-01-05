using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using SkiaSharp;
using System;
using System.Drawing;
using System.IO;


namespace draftio.views.components
{
    public class ColorPicker: Canvas
    {

        Flyout flyout = new Flyout();

        Canvas ColorArea = new Canvas();

        LinearGradientBrush paletteBrush;

        public delegate void ValueChangeDelegate();
        public ValueChangeDelegate? ValueChanged;

        public Avalonia.Media.Color SelectedColor { get; set; }

        public ColorPicker()
        {
            Background = Avalonia.Media.Brushes.Red;
            HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch;
            Height = 26;

            this.PointerPressed += ColorPicker_PointerPressed;
            setPaletteBrush();

            ColorArea.Width = 200;
            ColorArea.Height = 200;
            ColorArea.Background = paletteBrush;
            ColorArea.PointerPressed += ColorArea_PointerPressed;

            flyout.Content = ColorArea;

            this.ContextFlyout = flyout;
        }

        
        private void setPaletteBrush()
        {
            paletteBrush = new LinearGradientBrush();
            paletteBrush.GradientStops = new GradientStops
            {
                new GradientStop(Colors.Red, 0),
                new GradientStop(Colors.Blue, 0.5),
                new GradientStop(Colors.Green, 1),
            };
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

            SelectedColor = pixelColor;

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
    }
}
