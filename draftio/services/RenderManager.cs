using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using draftio.models.objects;
using draftio.models.objects.@base;
using draftio.models.structs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace draftio.services
{   
    // this service just will be used for rendering operations
    public class RenderManager
    {
        private Vector2 childMoveOffset = new Vector2(0,0);

        public RenderManager() { }

        public void Render(Panel Display, List<GObject> objects)
        {
            foreach (var obj in objects)
            {
                if(obj.ObjectType == models.enums.ObjectType.Canvas)
                {
                    CanvasObj canvasObj = (CanvasObj)obj;

                    Panel canvas = DrawCanvas(Display, obj);
                    
                    if (canvasObj != null && canvasObj.Children.Count() > 0)
                    {
                        Render(canvas, canvasObj.Children);
                    }
                }
                if(obj.ObjectType == models.enums.ObjectType.Text)
                {
                    TextObj textObj = (TextObj)obj;

                    DrawText(Display, textObj);
                }
            }
        }

        private Panel DrawCanvas (Panel Display, IObject obj)
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
            Rectangle canvasBackground = new Rectangle();
            Canvas.SetLeft(canvasBackground, 0);
            Canvas.SetTop(canvasBackground, 0);
            canvasBackground.Width = canvas.Width;
            canvasBackground.Height = canvas.Height;
            canvasBackground.Fill = Brushes.White;
            canvasBackground.Stroke = Brushes.Black;
            //canvasBackground.StrokeThickness = 1;

            canvas.Children.Add(canvasBackground);
            panel.Children.Add(canvas);


            Border border = new Border();
            border.Background = Brushes.Black;
            border.BorderThickness = Avalonia.Thickness.Parse("2");
            
            border.Padding = Avalonia.Thickness.Parse("0");
            border.Margin = Avalonia.Thickness.Parse("0");
            Canvas.SetLeft(border, obj.X - 2); 
            Canvas.SetTop(border, obj.Y - 2);
            border.Child = panel;

            Display.Children.Add(border);
            return canvas;
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

                //Display.Children.Add(stackPanel);
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
                textBlock.Margin = Avalonia.Thickness.Parse("0 5");
                textBlock.Width = obj.Width;
                textBlock.Height = obj.Height;
                textBlock.Text = obj.Text;
                textBlock.TextWrapping = TextWrapping.Wrap;
                canvas.Children.Add(textBlock);
                Display.Children.Add(canvas);
            }
        }

        
        public void RenderOverlay(Canvas Overlay, Point first, Point last, bool drawActive, bool moveActive, IObject? moveObject, Vector2 moveOffset)
        {
            Overlay.Children.Clear();

            if (drawActive)
            {
                Rectangle overlayActive = new Rectangle();
                Canvas.SetLeft(overlayActive, System.Math.Min(first.X, last.X));
                Canvas.SetTop(overlayActive, System.Math.Min(first.Y, last.Y));
                overlayActive.Width = Math.Abs(first.X - last.X);
                overlayActive.Height = Math.Abs(first.Y - last.Y);
                overlayActive.Fill = Brushes.Transparent;
                overlayActive.Stroke = Brushes.Black;
                overlayActive.StrokeThickness = 1;

                Overlay.Children.Add(overlayActive);
            }

            if(moveActive && moveObject != null)
            {
                childMoveOffset = new Vector2(0, 0);

                Rectangle overlayActive = new Rectangle();
                
                if(moveObject.Parent != null)
                {
                    childMoveOffset.X = moveObject.Parent.WorldX;
                    childMoveOffset.Y = moveObject.Parent.WorldY;
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
    }
}
