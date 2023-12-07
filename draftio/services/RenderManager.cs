using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using draftio.models.objects;
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
        public RenderManager() { }

        public void Render(Panel Display, List<IObject> objects)
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
    }
}
