using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Threading;
using draftio.models.dtos;
using draftio.services;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using SkiaSharp;
using System;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Xml.Linq;


namespace draftio.views.components
{
    public class ColorPicker: Canvas
    {
        public string Name { get; set; }

        ColorPickerManager colorPickerManager;

        TextBlock hexBlock = new TextBlock();

        StackPanel stack;
        Grid layout { get; set; }
        Grid labelrgb { get; set; }
        Grid inputrgb { get; set; }
        Grid inputhex { get; set; }

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

        byte R = 0;
        byte G = 0;
        byte B = 0;
        private string hex;
        public string Hex {
            get
            {
                return hex;
            }
            set
            {
                hex = value;
                UpdateHexState();
            }
        }
        string oldHex;

        // TextBox
        TextBox R_txt { get; set; } = new TextBox();
        TextBox G_txt { get; set; } = new TextBox();
        TextBox B_txt { get; set; } = new TextBox();
        TextBox A_txt { get; set; } = new TextBox();
        TextBox Hex_txt { get; set; } = new TextBox();

        TextBlock R_Block = new TextBlock();
        TextBlock G_Block = new TextBlock();
        TextBlock B_Block = new TextBlock();
        TextBlock Hex_Block = new TextBlock();

        Vertex[] vertices = new Vertex[]
        {
            new Vertex(new Vector2f(0, 0), SFML.Graphics.Color.White),
            new Vertex(new Vector2f(200, 0), SFML.Graphics.Color.Red),
            new Vertex(new Vector2f(200, 200), SFML.Graphics.Color.Black),
            new Vertex(new Vector2f(0, 200), SFML.Graphics.Color.Black)
        };

        public ColorDTO SelectedColor { get; set; } = new ColorDTO(255, 0, 0, 0);
        public ColorDTO SelectedBarColor { get; set; }

        public ColorPicker(string name)
        {
            Name = name;

            hexBlock.Text = "#" + hex;
            this.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center;
            this.VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center;
            hexBlock.Foreground = Brushes.White;
            hexBlock.Background = Brushes.Black;
            hexBlock.Opacity = 0.5;
            hexBlock.Padding = new Thickness(2);

            this.SizeChanged += ColorPicker_SizeChanged;

            Canvas.SetTop(hexBlock, 0);
            Canvas.SetLeft(hexBlock, 0);
            this.Children.Add(hexBlock);

            stack = new StackPanel();

            layout = new Grid();
            layout.ColumnDefinitions = new ColumnDefinitions("200, 5, 25");
            //layout.RowDefinitions = new RowDefinitions("200, 5, 50");
            stack.Children.Add(layout);

            labelrgb = new Grid();
            labelrgb.ColumnDefinitions = new ColumnDefinitions("70, 10, 70, 10, 70");
            labelrgb.Margin = new Thickness(0, 5, 0, 5);
            stack.Children.Add(labelrgb);

            inputrgb = new Grid();
            inputrgb.ColumnDefinitions = new ColumnDefinitions("70, 10, 70, 10, 70");
            inputrgb.Margin = new Thickness(0, 5, 0, 5);
            stack.Children.Add(inputrgb);

            inputhex = new Grid();
            inputhex.ColumnDefinitions = new ColumnDefinitions("25, *");
            inputhex.Margin = new Thickness(0, 5, 0, 5);
            stack.Children.Add(inputhex);

            colorPickerManager = App.GetService<ColorPickerManager>();

            Background = Avalonia.Media.Brushes.Red;
            HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch;
            Height = 26;

            this.PointerPressed += ColorPicker_PointerPressed;

            ColorArea.Width = 200;
            ColorArea.Height = 200;
            ColorArea.Focusable = true;

            ColorBar.Width = 25;
            ColorBar.Height = 200;
            ColorBar.Focusable = true;


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
            Grid.SetRow(BorderColorArea, 0);
            Grid.SetColumn(BorderColorBar, 2);
            Grid.SetRow(BorderColorBar, 0);

            BorderColorArea.Child = ColorArea;
            BorderColorBar.Child = ColorBar;

            layout.Children.Add(BorderColorArea);
            layout.Children.Add(BorderColorBar);

            setBarSelector();
            setBarBrush();

            flyout.Content = stack;
            flyout.ShowMode = FlyoutShowMode.Transient;

            this.ContextFlyout = flyout;


            setTextBoxes();
        }

        private void ColorPicker_SizeChanged(object? sender, SizeChangedEventArgs e)
        {
            double canvasWidth = this.Bounds.Width;
            double canvasHeight = this.Bounds.Height;

            // TextBlock'ın boyutlarını al
            double textBlockWidth = hexBlock.Bounds.Width;
            double textBlockHeight = hexBlock.Bounds.Height;

            // TextBlock'ı yatay ve dikey olarak ortala
            Canvas.SetLeft(hexBlock, (canvasWidth - textBlockWidth) / 2);
            Canvas.SetTop(hexBlock, (canvasHeight - textBlockHeight) / 2);

            hexBlock.Text = "#" + hex;

            UpdateHexState();
        }

        private void UpdateHexState()
        {
            hexBlock.Text = '#' + hex;

            R = SelectedColor.R;
            G = SelectedColor.G;
            B = SelectedColor.B;

            R_txt.Text = R.ToString();
            G_txt.Text = G.ToString();
            B_txt.Text = B.ToString();
        }

        private void setTextBoxes()
        {
            Grid.SetColumn(R_txt, 0);
            Grid.SetRow(R_txt, 0);
            inputrgb.Children.Add(R_txt);

            Grid.SetColumn(G_txt, 2);
            Grid.SetRow(G_txt, 0);
            inputrgb.Children.Add(G_txt);

            Grid.SetColumn(B_txt, 4);
            Grid.SetRow(B_txt, 0);
            inputrgb.Children.Add(B_txt);

            Grid.SetColumn(Hex_txt, 1);
            Grid.SetRow(Hex_txt, 0);
            inputhex.Children.Add(Hex_txt);

            //R_txt.TextChanged += R_TextChanged;
            R_txt.Focusable = true;
            R_txt.TextChanged += R_txt_TextChanged;
            R_txt.Name = "r";
            R_txt.Text = R.ToString();
            G_txt.TextChanged += R_txt_TextChanged;
            G_txt.Focusable = true;
            G_txt.Name = "g";
            G_txt.Text = G.ToString();
            B_txt.TextChanged += R_txt_TextChanged;
            B_txt.Focusable = true;
            B_txt.Name = "b";
            B_txt.Text = B.ToString();
            Hex_txt.TextChanged += R_txt_TextChanged;
            Hex_txt.Focusable = true;
            Hex_txt.Name = "hex";

            R_Block.Text = "Red";
            G_Block.Text = "Green";
            B_Block.Text = "Blue";
            Hex_Block.Text = "#";
            Hex_Block.VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center;
            Hex_Block.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center;

            Grid.SetColumn(R_Block, 0);
            Grid.SetRow(R_Block, 0);
            labelrgb.Children.Add(R_Block);

            Grid.SetColumn(G_Block, 2);
            Grid.SetRow(G_Block, 0);
            labelrgb.Children.Add(G_Block);

            Grid.SetColumn(B_Block, 4);
            Grid.SetRow(B_Block, 0);
            labelrgb.Children.Add(B_Block);

            Grid.SetColumn(Hex_Block, 0);
            Grid.SetRow(Hex_Block, 0);
            inputhex.Children.Add(Hex_Block);
        }
        
        private void R_txt_TextChanged(object? sender, TextChangedEventArgs e)
        {
            var txtBox = sender as TextBox;

            if(txtBox.Name == "r")
            {
                if (!String.IsNullOrEmpty(txtBox.Text))
                {
                    int v = CheckRGB(Int32.Parse(txtBox.Text));
                    R = (byte)v;
                    txtBox.Text = R.ToString();
                    
                    RgbToHex();
                }
            }

            if (txtBox.Name == "g")
            {
                if (!String.IsNullOrEmpty(txtBox.Text))
                {
                    int v = CheckRGB(Int32.Parse(txtBox.Text));
                    G = (byte)v;
                    txtBox.Text = G.ToString();
                    RgbToHex();
                }
            }

            if (txtBox.Name == "b")
            {
                if (!String.IsNullOrEmpty(txtBox.Text))
                {
                    int v = CheckRGB(Int32.Parse(txtBox.Text));
                    B = (byte)v;
                    txtBox.Text = B.ToString();
                    RgbToHex();
                }
            }

            if (txtBox.Name == "hex")
            {

                if(IsHex(txtBox.Text))
                {
                    if (!String.IsNullOrEmpty(txtBox.Text))
                    {
                        oldHex = Hex;
                        Hex = txtBox.Text;
                        
                        if(Hex.Length > 6)
                        {
                            Hex = Hex.Substring(0, 6);
                            oldHex = Hex;
                            txtBox.Text = Hex;
                        }
                        //if (Hex != txtBox.Text)
                        //    txtBox.Text = R.ToString();

                        HexToRgb();
                    }
                } else
                {
                    txtBox.Text = oldHex;
                    Hex = oldHex;
                }

            }
        }

        public bool IsHex(string text)
        {
            return text.All(c => "0123456789abcdefABCDEF\n".Contains(c));
        }

        private void RgbToHex()
        {
            byte[] byteArray = { R, G, B };
            Hex = string.Join("", byteArray.Select(b => b.ToString("X2")));
            Hex_txt.Text = Hex;
        }

        private String RbgToHex(byte r, byte g, byte b)
        {
            byte[] byteArray = { r, b, g };
            Hex = string.Join("", byteArray.Select(b => b.ToString("X2")));
            return Hex;
        }

        private void HexToRgb()
        {
            int max = 6;
            int len = Hex.Length;
            int difference = 0;

            if (Hex.Length < 6)
            {
                difference = max - len;
                for (int i = 0; i < difference; i++)
                {
                    Hex += "0";
                }
            } 
            else
            {
                Hex = Hex.Substring(0, Math.Min(Hex.Length, 6));
            }

            string rr = Hex.Substring(0, 2);
            string gg = Hex.Substring(2, 2);
            string bb = Hex.Substring(2 * 2, 2);

            R = (byte)Convert.ToInt32(rr, 16);
            G = (byte)Convert.ToInt32(gg, 16);
            B = (byte)Convert.ToInt32(bb, 16);

            R_txt.Text = R.ToString();
            G_txt.Text = G.ToString();
            B_txt.Text = B.ToString();
        }

        private int CheckRGB(int value)
        {
            if(value > 255)
            {
                return 255;
            }
            if(value < 0)
            {
                return 0;
            }
            return value;
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

            Hex_txt.Text = RbgToHex(SelectedColor.R, SelectedColor.G, SelectedColor.B);
            hexBlock.Text = "#" + Hex;

            if (ValueChanged != null)
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
