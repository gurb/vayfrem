using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using draftio.models.dtos;
using draftio.models.structs;
using draftio.services;
using SFML.Graphics;
using SFML.System;
using SkiaSharp;
using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;


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
        Grid labelhsv { get; set; }
        Grid inputhsv { get; set; }
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
        public Vector2 BarPosition { get; set; } = new Vector2(0, 0);
        Border colorSelector { get; set; } = new Border();
        public Vector2 ColorSelectPosition { get; set; } = new Vector2(0, 0);

        Avalonia.Controls.Image palette = new Avalonia.Controls.Image();
        Avalonia.Controls.Image colorBar = new Avalonia.Controls.Image();

        byte R = 0;
        byte G = 0;
        byte B = 0;

        int H = 0;
        int S = 0;
        int V = 0;

        private string hex;
        public string Hex
        {
            get { return hex;  }
            set { hex = value; UpdateHexState(); }
        }
        string oldHex;

        // TextBox
        TextBox R_txt { get; set; } = new TextBox();
        TextBox G_txt { get; set; } = new TextBox();
        TextBox B_txt { get; set; } = new TextBox();
        TextBox A_txt { get; set; } = new TextBox();
        TextBox Hex_txt { get; set; } = new TextBox();
        TextBox H_txt { get; set; } = new TextBox();
        TextBox S_txt { get; set; } = new TextBox();
        TextBox V_txt { get; set; } = new TextBox();


        TextBlock R_Block = new TextBlock();
        TextBlock G_Block = new TextBlock();
        TextBlock B_Block = new TextBlock();
        TextBlock Hex_Block = new TextBlock();
        TextBlock H_Block = new TextBlock();
        TextBlock S_Block = new TextBlock();
        TextBlock V_Block = new TextBlock();

        Vertex[] vertices = new Vertex[]
        {
            new Vertex(new Vector2f(0, 0), SFML.Graphics.Color.White),
            new Vertex(new Vector2f(255, 0), SFML.Graphics.Color.Red),
            new Vertex(new Vector2f(255, 255), SFML.Graphics.Color.Black),
            new Vertex(new Vector2f(0, 255), SFML.Graphics.Color.Black)
        };

        public ColorDTO SelectedColor { get; set; } = new ColorDTO(255, 0, 0, 0);
        public ColorDTO SelectedBarColor { get; set; }

        
        public ColorPicker(string name)
        {
            Name = name;

            hexBlock.Text = "#" + Hex;
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
            layout.ColumnDefinitions = new ColumnDefinitions("255, 5, 25");
            //layout.RowDefinitions = new RowDefinitions("200, 5, 50");
            stack.Children.Add(layout);

            labelrgb = new Grid();
            labelrgb.ColumnDefinitions = new ColumnDefinitions("85, 15, 85, 15, 85");
            labelrgb.Margin = new Thickness(0, 5, 0, 5);
            stack.Children.Add(labelrgb);

            inputrgb = new Grid();
            inputrgb.ColumnDefinitions = new ColumnDefinitions("85, 15, 85, 15, 85");
            inputrgb.Margin = new Thickness(0, 5, 0, 5);
            stack.Children.Add(inputrgb);

            labelhsv = new Grid();
            labelhsv.ColumnDefinitions = new ColumnDefinitions("85, 15, 85, 15, 85");
            labelhsv.Margin = new Thickness(0, 5, 0, 5);
            stack.Children.Add(labelhsv);

            inputhsv = new Grid();
            inputhsv.ColumnDefinitions = new ColumnDefinitions("85, 15, 85, 15, 85");
            inputhsv.Margin = new Thickness(0, 5, 0, 5);
            stack.Children.Add(inputhsv);

            inputhex = new Grid();
            inputhex.ColumnDefinitions = new ColumnDefinitions("25, *");
            inputhex.Margin = new Thickness(0, 5, 0, 5);
            stack.Children.Add(inputhex);

            colorPickerManager = App.GetService<ColorPickerManager>();

            Background = Avalonia.Media.Brushes.Red;
            HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch;
            Height = 26;

            this.PointerPressed += ColorPicker_PointerPressed;

            ColorArea.Width = 255;
            ColorArea.Height = 255;
            ColorArea.Focusable = true;

            ColorBar.Width = 25;
            ColorBar.Height = 255;
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
            setColorSelector();
        }

        public void SetColorPickerDTO(ColorPickerDTO dto)
        {
            SelectedColor = dto.Color;
            SelectedBarColor = dto.BarColor;
            ColorSelectPosition = dto.ColorSelectPosition;
            BarPosition = dto.BarPosition;

            vertices[1] = new Vertex(new Vector2f(255, 0), new SFML.Graphics.Color(SelectedBarColor.R, SelectedBarColor.G, SelectedBarColor.B, 255));
            colorPickerManager.UpdateTexture(Name, vertices);

            setPaletteBrush();

            Canvas.SetLeft(colorSelector, ColorSelectPosition.X);
            Canvas.SetTop(colorSelector, ColorSelectPosition.Y);

            Canvas.SetLeft(barSelector, 0);
            Canvas.SetTop(barSelector, BarPosition.Y);
        }

        private void setColorSelector()
        {
            colorSelector.Background = Brushes.Transparent;
            colorSelector.Width = 10;
            colorSelector.CornerRadius = new CornerRadius(2);
            colorSelector.Height = 10;
            colorSelector.BorderBrush = Brushes.Black;
            colorSelector.BorderThickness = new Thickness(2);

            

            Canvas.SetLeft(colorSelector, ColorSelectPosition.X);
            Canvas.SetTop(colorSelector, ColorSelectPosition.Y);
        }

        private void ColorPicker_SizeChanged(object? sender, SizeChangedEventArgs e)
        {
            hexBlock.Text = "#" + Hex;

            double canvasWidth = this.Bounds.Width;
            double canvasHeight = this.Bounds.Height;

            // TextBlock'ın boyutlarını al
            double textBlockWidth = hexBlock.Bounds.Width;
            double textBlockHeight = hexBlock.Bounds.Height;

            // TextBlock'ı yatay ve dikey olarak ortala
            Canvas.SetLeft(hexBlock, (canvasWidth - textBlockWidth) / 2);
            Canvas.SetTop(hexBlock, (canvasHeight - textBlockHeight) / 2);

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
            H_Block.Text = "Hue";
            S_Block.Text = "Saturation";
            V_Block.Text = "Value";


            Grid.SetColumn(H_txt, 0);
            Grid.SetRow(H_txt, 0);
            inputhsv.Children.Add(H_txt);

            Grid.SetColumn(S_txt, 2);
            Grid.SetRow(S_txt, 0);
            inputhsv.Children.Add(S_txt);

            Grid.SetColumn(V_txt, 4);
            Grid.SetRow(V_txt, 0);
            inputhsv.Children.Add(V_txt);

            H_txt.Focusable = true;
            H_txt.TextChanged += R_txt_TextChanged;
            H_txt.Name = "h";
            H_txt.Text = H.ToString();
            S_txt.TextChanged += R_txt_TextChanged;
            S_txt.Focusable = true;
            S_txt.Name = "s";
            S_txt.Text = S.ToString();
            V_txt.TextChanged += R_txt_TextChanged;
            V_txt.Focusable = true;
            V_txt.Name = "v";
            V_txt.Text = V.ToString();


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

            Grid.SetColumn(H_Block, 0);
            Grid.SetRow(H_Block, 0);
            labelhsv.Children.Add(H_Block);

            Grid.SetColumn(S_Block, 2);
            Grid.SetRow(S_Block, 0);
            labelhsv.Children.Add(S_Block);

            Grid.SetColumn(V_Block, 4);
            Grid.SetRow(V_Block, 0);
            labelhsv.Children.Add(V_Block);
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
                        oldHex = hex;
                        hex = txtBox.Text;
                        
                        if(hex.Length > 6)
                        {
                            hex = hex.Substring(0, 6);
                            oldHex = hex;
                            txtBox.Text = hex;
                        }

                        HexToRgb();
                    }
                } else
                {
                    txtBox.Text = oldHex;
                    hex = oldHex;
                }
            }

            if (txtBox.Name == "h")
            {
                if (!String.IsNullOrEmpty(txtBox.Text))
                {
                    int v = CheckHue(Int32.Parse(txtBox.Text));
                    H = (short)v;
                    txtBox.Text = H.ToString();

                    //RgbToHex();
                }
            }

            if (txtBox.Name == "s")
            {
                if (!String.IsNullOrEmpty(txtBox.Text))
                {
                    int v = CheckSV(Int32.Parse(txtBox.Text));
                    S = (short)v;
                    txtBox.Text = S.ToString();

                    //RgbToHex();
                }
            }

            if (txtBox.Name == "v")
            {
                if (!String.IsNullOrEmpty(txtBox.Text))
                {
                    int v = CheckSV(Int32.Parse(txtBox.Text));
                    V = (short)v;
                    txtBox.Text = V.ToString();

                    //RgbToHex();
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
            hex = string.Join("", byteArray.Select(x => x.ToString("X2")));
            Hex_txt.Text = hex;
        }

        private String RbgToHex(byte r, byte g, byte b)
        {
            byte[] byteArray = { r, g, b };
            hex = string.Join("", byteArray.Select(x => x.ToString("X2")));
            return hex;
        }

        private void HexToRgb()
        {
            int max = 6;
            int len = hex.Length;
            int difference = 0;

            if (hex.Length < 6)
            {
                difference = max - len;
                for (int i = 0; i < difference; i++)
                {
                    hex += "0";
                }
            } 
            else
            {
                hex = hex.Substring(0, Math.Min(hex.Length, 6));
            }

            string rr = hex.Substring(0, 2);
            string gg = hex.Substring(2, 2);
            string bb = hex.Substring(2 * 2, 2);

            R = (byte)Convert.ToInt32(rr, 16);
            G = (byte)Convert.ToInt32(gg, 16);
            B = (byte)Convert.ToInt32(bb, 16);

            R_txt.Text = R.ToString();
            G_txt.Text = G.ToString();
            B_txt.Text = B.ToString();

            HexToHsv();
        }

        private void HexToHsv()
        {
            double maxColor = Math.Max(R, Math.Max(G, B));
            double minColor = Math.Min(R, Math.Min(G, B));

            double normR = R;
            double normG = G;
            double normB = B;

            // Hue hesaplama

            if (maxColor == minColor)
            {
                H = 0;
            }
            else if (maxColor == R)
            {
                H = (int)((60 * ((normG - normB) / (maxColor - minColor)) + 360) % 360);
            }
            else if (maxColor == G)
            {
                H = (int)((60 * ((normB - normR) / (maxColor - minColor)) + 120) % 360);
            }
            else
            {
                H = (int)((60 * ((normR - normG) / (maxColor - minColor)) + 240) % 360);
            }

            // Saturation hesaplama
            S = (int)((maxColor == 0) ? 0 : ((maxColor - minColor) / maxColor) * 100);

            // Value hesaplama
            V = (int)(maxColor * 100) / 255;

            H_txt.Text = H.ToString();
            S_txt.Text = S.ToString();
            V_txt.Text = V.ToString();
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

        private int CheckHue(int value)
        {
            if (value > 360)
            {
                return 360;
            }
            if (value < 0)
            {
                return 0;
            }
            return value;
        }

        private int CheckSV(int value)
        {
            if (value > 100)
            {
                return 100;
            }
            if (value < 0)
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

            vertices[1] = new Vertex(new Vector2f(255, 0), new SFML.Graphics.Color(SelectedBarColor.R, SelectedBarColor.G, SelectedBarColor.B, 255));
            colorPickerManager.UpdateTexture(Name, vertices);

            Canvas.SetLeft(barSelector, 0);
            Canvas.SetTop(barSelector, y);

            BarPosition = new Vector2(0, y);
           
            setPaletteBrush();

            Avalonia.Media.Color pixelColorArea = GetPixelColor((int)ColorSelectPosition.X, (int)ColorSelectPosition.Y);
            SelectedColor = new ColorDTO(pixelColorArea);
            Hex_txt.Text = RbgToHex(SelectedColor.R, SelectedColor.G, SelectedColor.B);
            hexBlock.Text = "#" + hex;
            Background = new SolidColorBrush(pixelColorArea);

            if (ValueChanged != null)
            {
                ValueChanged.Invoke();
            }
            //ColorArea.Background = paletteBrush;
        }

        private void setBarSelector()
        {
            barSelector.Width = 25;
            barSelector.Height = 5;
            barSelector.Fill = Brushes.Black;

            ColorBar.Children.Add(barSelector);
            Canvas.SetLeft(barSelector, 0);
            Canvas.SetTop(barSelector, BarPosition.Y);
        }

        private void setPaletteBrush()
        {
            ColorArea.Children.Clear();

            //Avalonia.Media.Imaging.Bitmap avaloniaBitmap;

            byte[] pixels = colorPickerManager.GetTexture(Name).Texture.CopyToImage().Pixels;
            var avaloniaBitmap = new WriteableBitmap(new PixelSize(255, 255), new Vector(96, 96), Avalonia.Platform.PixelFormat.Rgba8888);

            using (var buffer = avaloniaBitmap.Lock())
            {
                // Copy pixel data from SFML to Avalonia
                Marshal.Copy(pixels, 0, buffer.Address, pixels.Length);
            }

            palette.Source = avaloniaBitmap;

            ColorArea.Children.Add(palette);
            ColorArea.Children.Add(colorSelector);

            Canvas.SetLeft(colorSelector, ColorSelectPosition.X - 5);
            Canvas.SetTop(colorSelector, ColorSelectPosition.Y - 5);

            Canvas.SetLeft(palette, 0);
            Canvas.SetTop(palette, 0);
            //paletteBrush = new LinearGradientBrush();
        }

        private void setBarBrush()
        {
            ColorBar.Children.Clear();

            //Avalonia.Media.Imaging.Bitmap avaloniaBitmap;

            byte[] pixels = colorPickerManager.colorBarPixels;
            var avaloniaBitmap = new WriteableBitmap(new PixelSize(25, 255), new Vector(96, 96), Avalonia.Platform.PixelFormat.Rgba8888);

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

            if (x > 255)
            {
                x = 255;
            }
            if (y > 255)
            {
                y = 255;
            }

            if (x < 0)
            {
                x = 0;
            }
            if (y < 0)
            {
                y = 0;
            }

            Avalonia.Media.Color pixelColor = GetPixelColor(x, y);

            Canvas.SetLeft(colorSelector, x - 5);
            Canvas.SetTop(colorSelector, y - 5);

            SelectedColor = new ColorDTO(pixelColor);
            ColorSelectPosition = new Vector2(x, y);

            Hex_txt.Text = RbgToHex(SelectedColor.R, SelectedColor.G, SelectedColor.B);
            hexBlock.Text = "#" + hex;

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
            var renderTarget = new RenderTargetBitmap(new PixelSize(25, 255), new Vector(96, 96));
            renderTarget.Render(ColorBar);

            SKBitmap skBitmap = RenderTargetBitmapToSKBitmap(renderTarget);

            SKColor color = skBitmap.GetPixel(x, y);

            return Avalonia.Media.Color.FromArgb(color.Alpha, color.Red, color.Green, color.Blue);
        }

    }
}
