using Avalonia.Controls;
using Avalonia.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace draftio.views.components
{
    public class Slider: Canvas
    {
        Canvas bar;
        Canvas behind;
        Canvas thumb;
        bool isDrag = false;
        Avalonia.Point CurrentPoint;

        public int Minimum { get; set; } = 0;
        public int Maximum { get; set; } = 100;
        public int Value { get; set; } = 0;


        private double percentage;


        public Slider()
        {
            HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch;
            Height = 30;
            Background = Brushes.White;

            this.SizeChanged += Slider_SizeChanged;
            this.PointerPressed += Slider_PointerPressed;
            this.PointerReleased += Slider_PointerReleased;
            this.PointerMoved += Slider_PointerMoved;

            SetBehind();
            SetBar();
            SetThumb();
        }

        private void Slider_PointerMoved(object? sender, Avalonia.Input.PointerEventArgs e)
        {
            Avalonia.Point point = e.GetPosition(sender as Control);

            int x = (int)point.X;
            int y = (int)point.Y;

            if (isDrag)
            {

                if (x <= 0) x = 0;
                if (x >= this.Bounds.Width - 20) x = (int)this.Bounds.Width - 20;

                percentage = (double)x / (this.Bounds.Width - 20);

                Canvas.SetLeft(thumb, x);
                Canvas.SetTop(thumb, 5);

                bar.Width = (int)(percentage * this.Bounds.Width);

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
            if(this.Bounds.Width <= 0)
            {
                return;
            }

            int x = (int)(percentage * this.Bounds.Width);

            Canvas.SetLeft(thumb, x);
            Canvas.SetTop(thumb, 5);

            bar.Width = (int)(percentage * this.Bounds.Width);
            behind.Width = this.Bounds.Width;

            Draw();
        }

        private void Draw()
        {
            Children.Clear();
            
            Children.Add(behind);
            Children.Add(bar);

            Children.Add(thumb);
        }

        private void SetBar()
        {
            bar = new Canvas();
            bar.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch;
            bar.Height = 5;
            bar.Background = Brushes.Green;

            Canvas.SetLeft(bar, 0);
            Canvas.SetTop(bar, 12.5);
            Children.Add(bar);

        }

        private void SetBehind()
        {
            behind = new Canvas();
            behind.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch;
            behind.Height = 5;
            behind.Background = Brushes.LightGray;

            Canvas.SetLeft(behind, 0);
            Canvas.SetTop(behind, 12.5);
            Children.Add(behind);
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
            Children.Add(thumb);
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
