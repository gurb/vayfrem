using Avalonia.Controls;
using Avalonia.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace draftio.views.components
{
    public class Slider: Grid
    {
        Canvas MainCanvas;
        Canvas bar;
        Canvas behind;
        Canvas thumb;

        TextBlock valueDisplay;

        bool isDrag = false;
        Avalonia.Point CurrentPoint;

        public int Minimum { get; set; } = 0;
        public int Maximum { get; set; } = 100;
        public int Value { get; set; } = 0;


        private double percentage;

        public delegate void ValueChangeDelegate();
        public ValueChangeDelegate? ValueChanged;

        public Slider()
        {
            ColumnDefinitions = new ColumnDefinitions("*, 50");

            Margin = new Avalonia.Thickness(5, 0, 5, 0);

            MainCanvas = new Canvas();
            HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch;
            Height = 30;
            Background = Brushes.White;

            valueDisplay = new TextBlock();
            valueDisplay.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch;
            valueDisplay.VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center;
            valueDisplay.Width = 50;
            valueDisplay.FontSize = 14;
            valueDisplay.Padding = new Avalonia.Thickness(10, 0, 10, 0);
            valueDisplay.Text = Value.ToString();

            MainCanvas.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch;
            MainCanvas.Height = 30;
            MainCanvas.Background = Brushes.White;

            Grid.SetColumn(MainCanvas, 0);
            Children.Add(MainCanvas);

            Grid.SetColumn(valueDisplay, 1);
            Children.Add(valueDisplay);

            this.SizeChanged += Slider_SizeChanged;
            this.PointerPressed += Slider_PointerPressed;
            this.PointerReleased += Slider_PointerReleased;
            this.PointerMoved += Slider_PointerMoved;
            //this.MainCanvas.SizeChanged += Slider_SizeChanged;



            SetBehind();
            SetBar();
            SetThumb();
        }

        private void Slider_PointerMoved(object? sender, Avalonia.Input.PointerEventArgs e)
        {
            Avalonia.Point point = e.GetPosition(sender as Control);

            // the 5 is margin offset which is came from MainCanvas Margin
            int x = (int)point.X;
            int y = (int)point.Y;

            if (isDrag)
            {

                if (x <= 0) x = 0;
                if (x >= MainCanvas.Bounds.Width - 20) x = (int)MainCanvas.Bounds.Width - 20;

                percentage = (double)x / (MainCanvas.Bounds.Width - 20);
                Value = ((int)(percentage * Maximum));
                valueDisplay.Text = Value.ToString();

                if (ValueChanged != null)
                {
                    ValueChanged.Invoke();
                }

                Canvas.SetLeft(thumb, x);
                Canvas.SetTop(thumb, 5);

                bar.Width = (int)(percentage * MainCanvas.Bounds.Width);

                Draw();
            }
        }

        private void Slider_PointerReleased(object? sender, Avalonia.Input.PointerReleasedEventArgs e)
        {
            isDrag = false;
        }

        private void Slider_PointerPressed(object? sender, Avalonia.Input.PointerPressedEventArgs e)
        {
        }

        private void Slider_SizeChanged(object? sender, SizeChangedEventArgs e)
        {
            Children.Clear(); 
            Grid.SetColumn(MainCanvas, 0);
            Children.Add(MainCanvas);

            Grid.SetColumn(valueDisplay, 1);
            Children.Add(valueDisplay);

            if (MainCanvas.Bounds.Width <= 0)
            {
                return;
            }

            int x = (int)(percentage * (MainCanvas.Bounds.Width - 20));

            Canvas.SetLeft(thumb, x);
            Canvas.SetTop(thumb, 5);

            bar.Width = (int)(percentage * (MainCanvas.Bounds.Width - 20));
            behind.Width = MainCanvas.Bounds.Width;

            Draw();
        }

        private void Draw()
        {
            MainCanvas.Children.Clear();

            MainCanvas.Children.Add(behind);
            MainCanvas.Children.Add(bar);

            MainCanvas.Children.Add(thumb);
        }

        private void SetBar()
        {
            bar = new Canvas();
            bar.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch;
            bar.Height = 5;
            bar.Background = Brushes.Green;
            bar.IsEnabled = false;

            Canvas.SetLeft(bar, 0);
            Canvas.SetTop(bar, 12.5);

            MainCanvas.Children.Add(bar);
        }

        private void SetBehind()
        {
            behind = new Canvas();
            behind.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch;
            behind.Height = 5;
            behind.Background = Brushes.LightGray;
            behind.PointerPressed += Behind_PointerPressed; ;

            Canvas.SetLeft(behind, 0);
            Canvas.SetTop(behind, 12.5);
            MainCanvas.Children.Add(behind);
        }

        private void Behind_PointerPressed(object? sender, Avalonia.Input.PointerPressedEventArgs e)
        {
            Avalonia.Point point = e.GetPosition(sender as Control);

            int x = (int)point.X;
            int y = (int)point.Y;

            if (x <= 0) x = 0;
            if (x >= MainCanvas.Bounds.Width - 20) x = (int)MainCanvas.Bounds.Width - 20;

            percentage = (double)x / (MainCanvas.Bounds.Width - 20);
            Value = ((int)(percentage * Maximum));
            valueDisplay.Text = Value.ToString();

            if (ValueChanged != null)
            {
                ValueChanged.Invoke();
            }

            Canvas.SetLeft(thumb, x);
            Canvas.SetTop(thumb, 5);

            bar.Width = (int)(percentage * MainCanvas.Bounds.Width);

            Draw();

        }

        private void SetThumb()
        {
            thumb = new Canvas();
            thumb.Width = 20;
            thumb.Height = 20;
            thumb.Background = Brushes.DarkGray;

            thumb.PointerPressed += Thumb_PointerPressed;
            thumb.PointerReleased += Thumb_PointerReleased;
            thumb.PointerMoved += Thumb_PointerMoved;

            Canvas.SetLeft(thumb, 0);
            Canvas.SetTop(thumb, 5);
            MainCanvas.Children.Add(thumb);
        }


        private void Thumb_PointerMoved(object? sender, Avalonia.Input.PointerEventArgs e)
        {
            Avalonia.Point point = e.GetPosition(sender as Control);

            int x = (int)point.X;
            int y = (int)point.Y;

            if (isDrag)
            {

            }

        }

        private void Thumb_PointerReleased(object? sender, Avalonia.Input.PointerReleasedEventArgs e)
        {
            Avalonia.Point point = e.GetPosition(sender as Control);

            int x = (int)point.X;
            int y = (int)point.Y;

            isDrag = false;
        }

        private void Thumb_PointerPressed(object? sender, Avalonia.Input.PointerPressedEventArgs e)
        {
            Avalonia.Point point = e.GetPosition(sender as Control);

            int x = (int)point.X;
            int y = (int)point.Y;

            isDrag = true;
        }
    }
}
