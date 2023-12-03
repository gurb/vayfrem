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

        public void Render(Canvas Display, List<IObject> objects)
        {
            foreach (var obj in objects)
            {
                if(obj.ObjectType == models.enums.ObjectType.Canvas)
                {
                    DrawCanvas(Display, obj);
                }
            }
        }


        private void DrawCanvas (Canvas Display, IObject obj)
        {
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
            canvasBackground.StrokeThickness = 1;

            canvas.Children.Add(canvasBackground);
            Display.Children.Add(canvas);
        }
    }
}
