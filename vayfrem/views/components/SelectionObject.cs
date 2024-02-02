using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using vayfrem.models.objects;
using vayfrem.viewmodels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vayfrem.views.components
{
    public class SelectionObject: Canvas
    {
        private Canvas MainDisplay;

        public Point OverlayPosition { get; set; }

        private DrawingViewModel ViewModel { get; set; }

        private Border Border = new Border();
        private Rectangle TopLeft = new Rectangle();
        private Rectangle TopRight = new Rectangle();
        private Rectangle TopCenter = new Rectangle();
        private Rectangle MiddleLeft = new Rectangle();
        private Rectangle MiddleRight = new Rectangle();
        private Rectangle BottomLeft = new Rectangle();
        private Rectangle BottomCenter = new Rectangle();
        private Rectangle BottomRight = new Rectangle();

        List<Rectangle> rects = new List<Rectangle>();


        public SelectionObject()
        {
            Width = 0;
            Height = 0;
            Background = Brushes.Transparent;
            InitBorder();
            InitRects();
            this.Cursor = new Avalonia.Input.Cursor(Avalonia.Input.StandardCursorType.BottomLeftCorner);

            this.PointerEntered += SelectionObject_PointerEntered;
        }

        private void SelectionObject_PointerEntered(object? sender, Avalonia.Input.PointerEventArgs e)
        {
            var test = 12;
        }

        public void SetMainDisplay(Canvas mainDisplay)
        {
            this.MainDisplay = mainDisplay;
        }

        public void SetViewModel(DrawingViewModel viewModel)
        {
            ViewModel = viewModel;
        }

        private void InitBorder()
        {
            Border = new Border();
            Border.BorderThickness = new Avalonia.Thickness(2);
            Border.BorderBrush = Brushes.Black;
        }

        private void InitRects()
        {
            TopLeft.Name = "TopLeft";
            rects.Add(TopLeft);
            TopCenter.Name = "TopCenter";
            rects.Add(TopCenter);
            TopRight.Name = "TopRight";
            rects.Add(TopRight);
            MiddleLeft.Name = "MiddleLeft";
            rects.Add(MiddleLeft);
            MiddleRight.Name = "MiddleRight";
            rects.Add(MiddleRight);
            BottomLeft.Name = "BottomLeft";
            rects.Add(BottomLeft);
            BottomCenter.Name = "BottomCenter";
            rects.Add(BottomCenter);
            BottomRight.Name = "BottomRight";
            rects.Add(BottomRight);

            Rectangle Sample = new Rectangle();
            
            Sample.Width = 10;
            Sample.Height = 10;
            Sample.StrokeThickness = 1;
            Sample.Stroke = Brushes.Black;

            foreach (var rect in rects)
            {
                rect.Width = Sample.Width;
                rect.Height = Sample.Height;
                rect.Fill = Brushes.White;
                rect.StrokeThickness = Sample.StrokeThickness;
                rect.Stroke = Sample.Stroke;
            }
        }

        public void Update(Point currentPosition, double zoom)
        {
            UpdateRects(zoom);
            var localPosition = currentPosition - this.OverlayPosition;

            CollisionDetectPoint(localPosition);
        }


        private void UpdateRects(double zoom)
        {
            double width = 10;
            double height = 10;
            double thickness = 1;
            double borderThickness = 2;

            if (zoom != 1)
            {

                double size = (10 / (double)zoom);
                width = size;
                height = size;
                thickness = 1 / (double)zoom;
                borderThickness = 2 / (double)zoom;
            }

            foreach (var rect in rects)
            {
                rect.Width = width;
                rect.Height = height;
                rect.StrokeThickness = thickness;
            }

            Border.BorderThickness = new Avalonia.Thickness(borderThickness);
        }

        public void Draw()
        {
            this.Children.Clear();

            // border
            Canvas.SetTop(Border, 0);
            Canvas.SetLeft(Border, 0);
            Border.Width = this.Width;
            Border.Height = this.Height;

            // top
            Canvas.SetTop(TopLeft, 0);
            Canvas.SetLeft(TopLeft, 0);

            Canvas.SetTop(TopCenter, 0);
            Canvas.SetLeft(TopCenter, Width / 2 - TopCenter.Width / 2);

            Canvas.SetTop(TopRight, 0);
            Canvas.SetRight(TopRight, 0);

            // center
            Canvas.SetTop(MiddleLeft, Height / 2 - MiddleLeft.Width / 2);
            Canvas.SetLeft(MiddleLeft, 0);

            Canvas.SetTop(MiddleRight, Height / 2 - MiddleRight.Width / 2);
            Canvas.SetRight(MiddleRight, 0);

            // bottom
            Canvas.SetBottom(BottomLeft, 0);
            Canvas.SetLeft(BottomLeft, 0);

            Canvas.SetBottom(BottomCenter, 0);
            Canvas.SetLeft(BottomCenter, Width / 2 - BottomCenter.Width / 2);

            Canvas.SetBottom(BottomRight, 0);
            Canvas.SetRight(BottomRight, 0);

            this.Children.Add(Border);
            foreach (var rect in rects)
            {
                this.Children.Add(rect);
            }
        }


        public void CollisionDetectPoint(Point mousePosition)
        {
            bool isCollide = false;

            if(ViewModel.IsScale)
            {
                return;
            }

            foreach (var obj in rects)
            {
                if (mousePosition.X >= obj.Bounds.TopLeft.X &&
                    mousePosition.X <= obj.Bounds.TopLeft.X + obj.Bounds.Width &&
                    mousePosition.Y >= obj.Bounds.TopLeft.Y &&
                    mousePosition.Y <= obj.Bounds.TopLeft.Y + obj.Bounds.Height)
                {
                    SetCursorAccordingToRectangle(obj);
                    ViewModel.IsOverScalePoint = true;
                    isCollide = true;
                    return;
                }
            }
            ViewModel.IsOverScalePoint = false;
            MainDisplay.Cursor = new Avalonia.Input.Cursor(Avalonia.Input.StandardCursorType.Arrow);
        }

        private void SetCursorAccordingToRectangle(Rectangle obj)
        {
            if (obj.Name == "TopLeft")
            {
                MainDisplay.Cursor = new Avalonia.Input.Cursor(Avalonia.Input.StandardCursorType.TopLeftCorner);
            }
            else if (obj.Name == "TopCenter")
            {
                MainDisplay.Cursor = new Avalonia.Input.Cursor(Avalonia.Input.StandardCursorType.SizeNorthSouth);
            }
            else if (obj.Name == "TopRight")
            {
                MainDisplay.Cursor = new Avalonia.Input.Cursor(Avalonia.Input.StandardCursorType.TopRightCorner);
            }
            else if (obj.Name == "MiddleLeft")
            {
                MainDisplay.Cursor = new Avalonia.Input.Cursor(Avalonia.Input.StandardCursorType.SizeWestEast);
            }
            else if (obj.Name == "MiddleRight")
            {
                MainDisplay.Cursor = new Avalonia.Input.Cursor(Avalonia.Input.StandardCursorType.SizeWestEast);
            }
            else if (obj.Name == "BottomLeft")
            {
                MainDisplay.Cursor = new Avalonia.Input.Cursor(Avalonia.Input.StandardCursorType.BottomLeftCorner);
            }
            else if (obj.Name == "BottomCenter")
            {
                MainDisplay.Cursor = new Avalonia.Input.Cursor(Avalonia.Input.StandardCursorType.SizeNorthSouth);
            }
            else if (obj.Name == "BottomRight")
            {
                MainDisplay.Cursor = new Avalonia.Input.Cursor(Avalonia.Input.StandardCursorType.BottomRightCorner);
            }

            ViewModel.GetOverScalePoint = obj.Name!;
        }
    }
}