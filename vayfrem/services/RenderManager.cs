using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using vayfrem.models.objects;
using vayfrem.models.objects.@base;
using vayfrem.models.objects.components;
using vayfrem.models.structs;
using vayfrem.viewmodels;
using vayfrem.views.components;
using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Platform;
using System.Net.Mime;
using Avalonia.Collections;

namespace vayfrem.services
{   
    // this service just will be used for rendering operations
    public class RenderManager
    {
        private Vector2 childMoveOffset = new Vector2(0,0);
        private readonly DrawingViewModel drawingViewModel;
        private readonly ToolManager toolManager;
        private readonly RecommendLineService recommendLineService;
        public Canvas? MainDisplay { get; set; }
        public double Zoom { get; set; } = 1;

        SelectionObject selectionObject = new SelectionObject();

        Avalonia.Media.Imaging.Bitmap imageLayer;

        Canvas ImageLayer;
        Border borderImage;
        Line line1;
        Line line2;

        public RenderManager() 
        {
            drawingViewModel = App.GetService<DrawingViewModel>();
            toolManager = App.GetService<ToolManager>();
            recommendLineService = App.GetService<RecommendLineService>();

            imageLayer = new Avalonia.Media.Imaging.Bitmap(AssetLoader.Open(new Uri("avares://vayfrem/assets/image-layer.png")));
            
        }

        public void SetMainDisplay(Canvas mainDisplay)
        {
            MainDisplay = mainDisplay;
            selectionObject.SetViewModel(drawingViewModel);
            selectionObject.SetMainDisplay(MainDisplay);
        }


        public void Render(Panel Display, List<GObject> objects, bool ignoreText = false)
        {
            foreach (var obj in objects)
            {
                if(obj.ObjectType == models.enums.ObjectType.Canvas)
                {
                    CanvasObj canvasObj = (CanvasObj)obj;

                    Panel canvas = DrawCanvas(Display, canvasObj);
                    
                    if (canvasObj != null && canvasObj.Children.Count() > 0)
                    {
                        Render(canvas, canvasObj.Children, ignoreText);
                    }
                }
                if(obj.ObjectType == models.enums.ObjectType.Text && !ignoreText)
                {
                    TextObj textObj = (TextObj)obj;

                    DrawText(Display, textObj);
                }
                if(obj.ObjectType == models.enums.ObjectType.QuadraticBC)
                {
                    QuadraticBCObj quadraticBCObj = (QuadraticBCObj)obj;

                    DrawQBC(Display, quadraticBCObj);
                }
                if (obj.ObjectType == models.enums.ObjectType.Button)
                {
                    ButtonObj buttonObj = (ButtonObj)obj;

                    DrawButton(Display, buttonObj);
                }
                if (obj.ObjectType == models.enums.ObjectType.Svg)
                {
                    SvgObj svgObj = (SvgObj)obj;

                    DrawSvg(Display, svgObj);
                }
                if (obj.ObjectType == models.enums.ObjectType.Image)
                {
                    ImageObj imgObj = (ImageObj)obj;

                    DrawImage(Display, imgObj);
                }
            }
        }

        private Panel DrawCanvas (Panel Display, CanvasObj obj)
        {
            Border boxShadow = new Border();
            boxShadow.Padding = new Thickness(0);
            Canvas.SetLeft(boxShadow, obj.X);
            Canvas.SetTop(boxShadow, obj.Y);

            if (obj.IsBoxShadowActive)
            {
                boxShadow.BoxShadow = new BoxShadows(new BoxShadow
                {
                    Blur = obj.BoxShadow.Blur,
                    OffsetX = obj.BoxShadow.HOffset,
                    OffsetY = obj.BoxShadow.VOffset,
                    IsInset = obj.BoxShadow.Inset,
                    Spread = obj.BoxShadow.Spread,
                    Color = Color.Parse("#"+obj.BoxShadowColor.ToHex())
                });
                boxShadow.Background = Brushes.Transparent;
                boxShadow.CornerRadius = new CornerRadius(obj.BorderRadius);
                boxShadow.BorderThickness = new Thickness(0);
            }

            Canvas panel = new Canvas();

            
            Canvas.SetLeft(panel, 0);
            Canvas.SetTop(panel, 0);
            panel.Margin = new Thickness(0);
            panel.Width = obj.Width;
            panel.Height = obj.Height;


            Border border = new Border();
            if(obj.BorderDTO.Relative)
            {
                border.BorderThickness = Avalonia.Thickness.Parse(obj.BorderDTO.GetRelative());
            }
            else
            {
                border.BorderThickness = new Thickness(obj.BorderDTO.Thickness);
            }
            border.BorderBrush = Brushes.Black;
            border.Width = panel.Width;
            border.Height = panel.Height;
            border.Background = new SolidColorBrush(obj.BackgroundColor.ToColor(), obj.Opacity / 255.0);
            Canvas.SetLeft(border, 0);
            Canvas.SetTop(border, 0);
            panel.Children.Add(border);


            Canvas drawing = new Canvas();
            drawing.Background = Brushes.Transparent;
            drawing.ClipToBounds = true;

            if (obj.BorderDTO.Relative)
            {
                drawing.Width = panel.Width - obj.BorderDTO.LeftThickness + obj.BorderDTO.RightThickness;
                drawing.Height = panel.Height - obj.BorderDTO.TopThickness + obj.BorderDTO.BottomThickness;
                Canvas.SetLeft(drawing, obj.BorderDTO.LeftThickness);
                Canvas.SetTop(drawing, obj.BorderDTO.TopThickness);
            }
            else
            {
                drawing.Width = panel.Width - obj.BorderDTO.Thickness * 2;
                drawing.Height = panel.Height - obj.BorderDTO.Thickness * 2;
                Canvas.SetLeft(drawing, obj.BorderDTO.Thickness);
                Canvas.SetTop(drawing, obj.BorderDTO.Thickness);
            }

            panel.Children.Add(drawing);


            boxShadow.Child = panel;
            Display.Children.Add(boxShadow);

            return drawing;
        }

        public void DrawSvg(Panel Display, SvgObj obj)
        {
            Avalonia.Svg.Svg svg;
            Uri uri = new System.Uri(obj.Path);

            svg = new Avalonia.Svg.Svg(uri);
            svg.Path = obj.Path;
            svg.Width = obj.Width;
            svg.Height = obj.Height;
            

            Canvas.SetLeft(svg, obj.X);
            Canvas.SetTop(svg, obj.Y);

            Display.Children.Add(svg);
        }

        public void DrawImage(Panel Display, ImageObj obj)
        {
            RelativePanel panel = new RelativePanel();
            Canvas.SetLeft(panel, obj.X);
            Canvas.SetTop(panel, obj.Y);
            panel.Width = obj.Width;
            panel.Height = obj.Height;


            if(obj.Base64 != null)
            {
                Image image = new Image();
                image.Source = obj.Image.Source;
                image.Width = obj.Width;
                image.Height = obj.Height;

                Canvas.SetLeft(image, 0);
                Canvas.SetTop(image, 0);
                panel.Children.Add(image);
            }
            else
            {
                Canvas canvas = new Canvas();
                Canvas.SetLeft(canvas, obj.X);
                Canvas.SetTop(canvas, obj.Y);
                canvas.Width = obj.Width;
                canvas.Height = obj.Height;

                Line line1 = new Line();
                line1.StrokeThickness = 5;
                line1.Stroke = Brushes.Black;
                line1.StartPoint = new Point(20, 20);
                line1.EndPoint = new Point(obj.Width - 20, obj.Height - 20);

                Line line2 = new Line();
                line2.StrokeThickness = 5;
                line2.Stroke = Brushes.Black;
                line2.StartPoint = new Point(obj.Width - 20, 20);
                line2.EndPoint = new Point(20, obj.Height - 20);

                canvas.Children.Add(line1);
                canvas.Children.Add(line2);
                //canvasBackground.StrokeThickness = 1;

                //panel.Children.Add(canvas);

                Border border = new Border();
                border.Background = new SolidColorBrush(obj.BackgroundColor.ToColor(), obj.Opacity / 255.0);
                border.BorderThickness = Avalonia.Thickness.Parse("2"); ;
                border.BorderBrush = Brushes.Black;

                border.Padding = Avalonia.Thickness.Parse("0");
                border.Margin = Avalonia.Thickness.Parse("0");
                Canvas.SetLeft(border, obj.X);
                Canvas.SetTop(border, obj.Y);
                border.Child = canvas;
                panel.Children.Add(border);
            }
            

            Display.Children.Add(panel);
        }

        public void DrawQBC(Panel Display, QuadraticBCObj obj)
        {
            Path path = new Path();

            path.Stroke = new SolidColorBrush(obj.BorderColor.ToColor());
            path.StrokeThickness = obj.BorderDTO.Thickness;
            path.Fill = new SolidColorBrush(obj.BackgroundColor.ToColor(), obj.Opacity / 255.0);
            path.Data = new PathGeometry
            {
                Figures = new PathFigures
                {
                    new PathFigure
                    {
                        StartPoint = new Point(obj.StartPoint.X, obj.StartPoint.Y),
                        Segments = new PathSegments
                        {
                            new QuadraticBezierSegment
                            {
                                Point1 = new Point(obj.Point1.X, obj.Point1.Y),
                                Point2 = new Point(obj.Point2.X, obj.Point2.Y)
                            }
                        }
                    }
                }
            };

            Display.Children.Add(path);
        }

        private void DrawText(Panel Display, TextObj obj)
        {
            if(obj.IsEditMode)
            {
                RelativePanel panel = new RelativePanel();
                Canvas.SetLeft(panel, obj.X);
                Canvas.SetTop(panel, obj.Y);
                panel.Width = obj.Width;
                panel.Height = obj.Height;

                Canvas canvas = new Canvas();
                Canvas.SetLeft(canvas, obj.X);
                Canvas.SetTop(canvas, obj.Y);
                canvas.Width = obj.Width;
                canvas.Height = obj.Height;

                //RelativePanel stackPanel = new RelativePanel();
                //Canvas.SetLeft(stackPanel, obj.X);
                //Canvas.SetTop(stackPanel, obj.Y);
                //stackPanel.Width = obj.Width;
                //stackPanel.Height = obj.Height;

                TextBox textBox = new TextBox();
                Canvas.SetLeft(textBox, 0);
                Canvas.SetTop(textBox, 0);
                textBox.Width = obj.Width;
                textBox.Height = obj.Height;
                textBox.FontFamily = new FontFamily(obj.FontFamily!);
                textBox.FontSize = obj.FontSize;
                textBox.FontWeight = (FontWeight)obj.FontWeight;
                textBox.Foreground = new SolidColorBrush(obj.FontColor.ToColor());
                textBox.TextWrapping = TextWrapping.Wrap;
                textBox.Background = Brushes.Gray;
                textBox.Text = obj.Text;
                textBox.TextChanged += obj.TextBox_TextChanged;

                canvas.Children.Add(textBox);
                panel.Children.Add(canvas);
                //stackPanel.Children.Add(textBox);

                Border border = new Border();
                border.Background = Brushes.Black;
                border.BorderThickness = Avalonia.Thickness.Parse("2");

                border.Padding = Avalonia.Thickness.Parse("0");
                border.Margin = Avalonia.Thickness.Parse("0");
                Canvas.SetLeft(border, obj.X - 2);
                Canvas.SetTop(border, obj.Y - 2);
                border.Child = panel;

                Display.Children.Add(border);
            }
            else
            {
                Canvas canvas = new Canvas();
                Canvas.SetLeft(canvas, obj.X);
                Canvas.SetTop(canvas, obj.Y);
                canvas.Width = obj.Width;
                canvas.Height = obj.Height;

                TextBlock textBlock = new TextBlock();

                Canvas.SetLeft(textBlock, 0);
                Canvas.SetTop(textBlock, 0);

                if (obj.ContentAlignment == models.enums.ContentAlignment.Left)
                {
                    textBlock.TextAlignment = Avalonia.Media.TextAlignment.Left;
                }
                else if (obj.ContentAlignment == models.enums.ContentAlignment.Center)
                {
                    textBlock.TextAlignment = Avalonia.Media.TextAlignment.Center;
                }
                else if (obj.ContentAlignment == models.enums.ContentAlignment.Right)
                {
                    textBlock.TextAlignment = Avalonia.Media.TextAlignment.Right;
                }

                textBlock.Margin = Avalonia.Thickness.Parse("0 5");
                textBlock.Width = obj.Width;
                textBlock.Height = obj.Height;
                textBlock.Text = obj.Text;
                textBlock.FontSize = obj.FontSize;
                textBlock.FontWeight = (FontWeight)obj.FontWeight;
                textBlock.FontFamily = new FontFamily(obj.FontFamily);
                textBlock.Foreground = new SolidColorBrush(obj.FontColor.ToColor());
                textBlock.TextWrapping = TextWrapping.Wrap;
                canvas.Children.Add(textBlock);
                Display.Children.Add(canvas);
            }
        }

        private void DrawButton(Panel Display, ButtonObj obj)
        {
            if (obj.IsEditMode)
            {
                RelativePanel panel = new RelativePanel();
                Canvas.SetLeft(panel, obj.X);
                Canvas.SetTop(panel, obj.Y);
                panel.Width = obj.Width;
                panel.Height = obj.Height;

                Canvas canvas = new Canvas();
                Canvas.SetLeft(canvas, obj.X);
                Canvas.SetTop(canvas, obj.Y);
                canvas.Width = obj.Width;
                canvas.Height = obj.Height;

                //RelativePanel stackPanel = new RelativePanel();
                //Canvas.SetLeft(stackPanel, obj.X);
                //Canvas.SetTop(stackPanel, obj.Y);
                //stackPanel.Width = obj.Width;
                //stackPanel.Height = obj.Height;

                TextBox textBox = new TextBox();
                Canvas.SetLeft(textBox, 0);
                Canvas.SetTop(textBox, 0);
                textBox.Width = obj.Width;
                textBox.Height = obj.Height;
                textBox.FontFamily = new FontFamily(obj.FontFamily!);
                textBox.FontSize = obj.FontSize;
                textBox.FontWeight = (FontWeight)obj.FontWeight;
                textBox.Foreground = new SolidColorBrush(obj.FontColor.ToColor());
                textBox.TextWrapping = TextWrapping.Wrap;
                textBox.Background = Brushes.Gray;
                textBox.Text = obj.Text;
                textBox.TextChanged += obj.TextBox_TextChanged;

                canvas.Children.Add(textBox);
                panel.Children.Add(canvas);
                //stackPanel.Children.Add(textBox);

                Border border = new Border();
                border.Background = new SolidColorBrush(obj.BackgroundColor.ToColor(), obj.Opacity / 255.0);
                border.BorderBrush = new SolidColorBrush(obj.BorderColor.ToColor());

                if(obj.BorderDTO.Relative)
                {
                    border.BorderThickness = Avalonia.Thickness.Parse(obj.BorderDTO.GetRelative());
                } 
                else
                {
                    border.BorderThickness = Avalonia.Thickness.Parse(obj.BorderDTO.Thickness.ToString());
                }
                
                border.CornerRadius = new CornerRadius(obj.BorderRadius);

                border.Padding = Avalonia.Thickness.Parse("0");
                border.Margin = Avalonia.Thickness.Parse("0");
                Canvas.SetLeft(border, obj.X);
                Canvas.SetTop(border, obj.Y);
                border.Child = panel;

                Display.Children.Add(border);
            }
            else
            {
                RelativePanel panel = new RelativePanel();
                Canvas.SetLeft(panel, obj.X);
                Canvas.SetTop(panel, obj.Y);
                panel.Width = obj.Width;
                panel.Height = obj.Height;

                Canvas canvas = new Canvas();
                Canvas.SetLeft(canvas, obj.X);
                Canvas.SetTop(canvas, obj.Y);
                canvas.Width = obj.Width;
                canvas.Height = obj.Height;

                TextBlock textBlock = new TextBlock();
                
                // alignment according to gobject
                if(obj.TextAlignment == models.enums.TextAlignment.TopLeft)
                {
                    Canvas.SetLeft(textBlock, 0);
                    Canvas.SetTop(textBlock, 0);
                }
                else if(obj.TextAlignment == models.enums.TextAlignment.TopCenter)
                {
                    textBlock.TextAlignment = Avalonia.Media.TextAlignment.Center;
                    Canvas.SetTop(textBlock, 0);
                }
                else if(obj.TextAlignment == models.enums.TextAlignment.TopRight)
                {
                    textBlock.TextAlignment = Avalonia.Media.TextAlignment.Right;
                    Canvas.SetTop(textBlock, 0);
                }
                else if(obj.TextAlignment == models.enums.TextAlignment.MiddleLeft)
                {
                    Canvas.SetLeft(textBlock, 0);
                    Canvas.SetTop(textBlock, obj.Height / 2.0  - obj.FontSize / 2.0);
                }
                else if (obj.TextAlignment == models.enums.TextAlignment.MiddleCenter)
                {
                    Canvas.SetLeft(textBlock, 0);
                    textBlock.TextAlignment = Avalonia.Media.TextAlignment.Center;
                    Canvas.SetTop(textBlock, obj.Height / 2.0 - obj.FontSize / 2.0);
                }
                else if (obj.TextAlignment == models.enums.TextAlignment.MiddleRight)
                {
                    Canvas.SetRight(textBlock, 0);
                    textBlock.TextAlignment = Avalonia.Media.TextAlignment.Right;
                    Canvas.SetTop(textBlock, obj.Height / 2.0 - obj.FontSize / 2.0);
                }
                if (obj.TextAlignment == models.enums.TextAlignment.BottomLeft)
                {
                    Canvas.SetLeft(textBlock, 0);
                    Canvas.SetBottom(textBlock, 0);
                }
                if (obj.TextAlignment == models.enums.TextAlignment.BottomCenter)
                {
                    Canvas.SetLeft(textBlock, 0);
                    textBlock.TextAlignment = Avalonia.Media.TextAlignment.Center;
                    Canvas.SetBottom(textBlock, 0);
                }
                if (obj.TextAlignment == models.enums.TextAlignment.BottomRight)
                {
                    Canvas.SetLeft(textBlock, 0);
                    textBlock.TextAlignment = Avalonia.Media.TextAlignment.Right;
                    Canvas.SetBottom(textBlock, 0);
                }

                //textBlock.Margin = Avalonia.Thickness.Parse("0 5");
                textBlock.Width = obj.Width;
                textBlock.Height = obj.FontSize;
                textBlock.Text = obj.Text;
                textBlock.FontSize = obj.FontSize;
                textBlock.FontFamily = new FontFamily(obj.FontFamily);
                textBlock.Foreground = new SolidColorBrush(obj.FontColor.ToColor());
                textBlock.FontWeight = (FontWeight)obj.FontWeight;
                //textBlock.Background = Brushes.Aqua;
                textBlock.TextWrapping = TextWrapping.Wrap;
                //textBlock.TextAlignment = obj.TextAlignmentConverter(obj.TextAlignment);
                textBlock.VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center;
                canvas.Children.Add(textBlock);

                Border border = new Border();
                border.Background = new SolidColorBrush(obj.BackgroundColor.ToColor(), obj.Opacity / 255.0);
                border.BorderBrush = new SolidColorBrush(obj.BorderColor.ToColor());
                if (obj.BorderDTO.Relative)
                {
                    border.BorderThickness = Avalonia.Thickness.Parse(obj.BorderDTO.GetRelative());
                }
                else
                {
                    border.BorderThickness = Avalonia.Thickness.Parse(obj.BorderDTO.Thickness.ToString());
                }
                border.CornerRadius = new CornerRadius(obj.BorderRadius);

                border.Padding = Avalonia.Thickness.Parse("0");
                border.Margin = Avalonia.Thickness.Parse("0");
                Canvas.SetLeft(border, obj.X);
                Canvas.SetTop(border, obj.Y);

                border.Child = canvas;
                panel.Children.Add(border);

                Display.Children.Add(panel);
            }
        }

        // last equals currentPosition ****
        public void RenderOverlay(Canvas Overlay, Avalonia.Point first, Avalonia.Point last, bool drawActive, bool moveActive, GObject? moveObject, Vector2 moveOffset)
        {
            Overlay.Children.Clear();

            if (drawActive)
            {
                if(toolManager.SelectedToolOption == models.enums.ToolOption.Rect || toolManager.SelectedToolOption == models.enums.ToolOption.Text)
                {
                    Rectangle overlayActive = new Rectangle();
                    Canvas.SetLeft(overlayActive, (int)System.Math.Min(first.X, last.X));
                    Canvas.SetTop(overlayActive, (int)System.Math.Min(first.Y, last.Y));
                    overlayActive.Width = (int)Math.Abs(first.X - last.X);
                    overlayActive.Height = (int)Math.Abs(first.Y - last.Y);
                    overlayActive.Fill = Brushes.Transparent;
                    overlayActive.Stroke = Brushes.Black;
                    overlayActive.StrokeThickness = 1;

                    Overlay.Children.Add(overlayActive);
                }

                if(toolManager.SelectedToolOption == models.enums.ToolOption.QBC)
                {
                    Path path = new Path();

                    Vector2 p1 = new Vector2((first.X + last.X) / 2, (first.Y + last.Y) / 2);

                    path.Stroke = Brushes.Black;
                    path.StrokeThickness = 1.0;
                    path.Fill = Brushes.Black;
                    path.Data = new PathGeometry
                    {
                        Figures = new PathFigures
                        {
                            new PathFigure
                            {
                                StartPoint = new Point(first.X, first.Y),
                                Segments = new PathSegments
                                {
                                    new QuadraticBezierSegment
                                    {
                                        Point1 = new Point(p1.X, p1.Y),
                                        Point2 = new Point(last.X, last.Y)
                                    }
                                }
                            }
                        }
                    };

                    Overlay.Children.Add(path);
                }
            }

            if(moveActive && moveObject != null)
            {
                childMoveOffset = new Vector2(0, 0);

                if (moveObject.ObjectType == models.enums.ObjectType.QuadraticBC)
                {

                    QuadraticBCObj origin = (QuadraticBCObj)moveObject;
                    QuadraticBCObj quadraticBCObj = origin.Copy();


                    Path path = new Path();

                    Vector2 p1 = new Vector2((first.X + last.X) / 2, (first.Y + last.Y) / 2);

                    if (moveObject.Parent != null)
                    {
                        childMoveOffset.X = (int)moveObject.Parent.WorldX + (int)moveObject.Parent.BorderOffsetX;
                        childMoveOffset.Y = (int)moveObject.Parent.WorldY + (int)moveObject.Parent.BorderOffsetY;
                    }

                    moveOffset = new Vector2(first.X - quadraticBCObj.StartPoint.X, first.Y - quadraticBCObj.StartPoint.Y);
                    Vector2 old = new Vector2(quadraticBCObj.StartPoint);

                    quadraticBCObj.StartPoint.X = (int)(last.X - moveOffset.X);
                    quadraticBCObj.StartPoint.Y = (int)(last.Y - moveOffset.Y);
                    Vector2 delta = old - quadraticBCObj.StartPoint;

                    quadraticBCObj.Point1 = quadraticBCObj.Point1 - delta;
                    quadraticBCObj.Point2 = quadraticBCObj.Point2 - delta;

                    path.Stroke = Brushes.Aqua;
                    path.StrokeThickness = 1.0;
                    path.Fill = Brushes.Aqua;
                    path.Opacity = 0.4;
                    path.Data = new PathGeometry
                    {
                        Figures = new PathFigures
                        {
                            new PathFigure
                            {
                                StartPoint = new Point(quadraticBCObj.StartPoint.X, quadraticBCObj.StartPoint.Y) + new Point(childMoveOffset.X, childMoveOffset.Y),
                                Segments = new PathSegments
                                {
                                    new QuadraticBezierSegment
                                    {
                                        Point1 = new Avalonia.Point(quadraticBCObj.Point1.X, quadraticBCObj.Point1.Y) + new Point(childMoveOffset.X, childMoveOffset.Y),
                                        Point2 = new Avalonia.Point(quadraticBCObj.Point2.X, quadraticBCObj.Point2.Y) + new Point(childMoveOffset.X, childMoveOffset.Y)
                                    }
                                }
                            }
                        }
                    };

                    Overlay.Children.Add(path);
                }
                else
                {
                    Rectangle overlayActive = new Rectangle();

                    if (moveObject.Parent != null)
                    {
                        childMoveOffset.X = (int)moveObject.Parent.WorldX + (int)moveObject.Parent.BorderOffsetX;
                        childMoveOffset.Y = (int)moveObject.Parent.WorldY + (int)moveObject.Parent.BorderOffsetY;
                    }

                    Canvas.SetLeft(overlayActive, (childMoveOffset.X + last.X) - (moveOffset.X));
                    Canvas.SetTop(overlayActive, (childMoveOffset.Y + last.Y) - (moveOffset.Y));
                    overlayActive.Width = moveObject.Width;
                    overlayActive.Height = moveObject.Height;
                    overlayActive.Fill = Brushes.Aqua;
                    overlayActive.Opacity = 0.4;
                    overlayActive.Stroke = Brushes.Aqua;
                    overlayActive.StrokeThickness = 1;

                    Overlay.Children.Add(overlayActive);
                }
            }

            if(moveActive && drawingViewModel.CurrentFile != null)
            {
                foreach (var x in recommendLineService.XAxis)
                {
                    Line line = new Line
                    {
                        StartPoint = new Point(x, 0),
                        EndPoint = new Point(x, drawingViewModel.CurrentFile.PageHeight),
                        Stroke = Brushes.Green,
                        StrokeThickness = 1,
                        StrokeDashArray = new AvaloniaList<double> { 4, 2 }
                    };

                    Overlay.Children.Add(line);
                }
                foreach (var y in recommendLineService.YAxis)
                {
                    Line line = new Line
                    {
                        StartPoint = new Point(0, y),
                        EndPoint = new Point(drawingViewModel.CurrentFile.PageWidth, y),
                        Stroke = Brushes.Green,
                        StrokeThickness = 1,
                        StrokeDashArray = new AvaloniaList<double> { 4, 2 }
                    };

                    Overlay.Children.Add(line);
                }
            }

            if(
                drawingViewModel.CurrentFile != null && 
                drawingViewModel.CurrentFile.Selection!.SelectedObject != null && 
                drawingViewModel.CurrentFile.Selection!.SelectedObject.ObjectType != models.enums.ObjectType.QuadraticBC)
            {
                GObject selected = drawingViewModel.CurrentFile.Selection!.SelectedObject;
                selectionObject.IsVisible = true;
                selectionObject.Width = selected.Width;
                selectionObject.Height = selected.Height;
                Canvas.SetLeft(selectionObject, selected.WorldX);
                Canvas.SetTop(selectionObject, selected.WorldY);

                selectionObject.OverlayPosition = new Point(selected.WorldX, selected.WorldY);

                selectionObject.Update(last, Zoom);

                selectionObject.Draw();

                Overlay.Children.Add(selectionObject);
            }
            else if(
                drawingViewModel.CurrentFile != null &&
                drawingViewModel.CurrentFile.Selection!.SelectedObject != null &&
                drawingViewModel.CurrentFile.Selection!.SelectedObject.ObjectType == models.enums.ObjectType.QuadraticBC
            )
            {

            }
            else
            {
                selectionObject.IsVisible = false;
            }
        } 
    }
}