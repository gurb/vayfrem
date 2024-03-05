using Avalonia.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using vayfrem.models.objects.@base;
using vayfrem.models.structs;
using vayfrem.viewmodels;


namespace vayfrem.services
{
    public class RecommendLineService
    {

        public List<int> XAxis { get; set; }
        public List<int> YAxis { get; set; }

        public RecommendLineService() 
        {
            XAxis = new List<int>();
            YAxis = new List<int>();
        }

       

        public void Check(DrawingViewModel drawingViewModel, Avalonia.Point first, Avalonia.Point current, Vector2 moveOffset)
        {
            YAxis.Clear();
            XAxis.Clear();

            Vector2 childMoveOffset = new Vector2(0,0);

            var selected = drawingViewModel.SelectedObject;

            if(selected != null && drawingViewModel.CurrentFile != null)
            {
                if (selected.Parent != null)
                {
                    childMoveOffset.X = (int)selected.Parent.WorldX + (int)selected.Parent.BorderOffsetX;
                    childMoveOffset.Y = (int)selected.Parent.WorldY + (int)selected.Parent.BorderOffsetY;
                }

                int x = (int)((childMoveOffset.X + current.X) - (moveOffset.X));
                int y = (int)((childMoveOffset.Y + current.Y) - (moveOffset.Y));
                int width = (int)selected.Width;
                int height = (int)selected.Height;

                Vector2 TopLeft = new Vector2(x, y);
                Vector2 BottomLeft = new Vector2(x, y + height);
                Vector2 TopRight = new Vector2(x + width, y);
                Vector2 BottomRight = new Vector2(x + width, y + height);


                foreach (var obj in drawingViewModel.CurrentFile.AllObjects)
                {
                    if(obj != drawingViewModel.SelectedObject)
                    {
                        // right edge of selected
                        if((int)obj.TopLeft.X == (int)TopRight.X && 
                           (int)obj.TopLeft.X == (int)BottomRight.X && 
                           (int)obj.BottomLeft.X == (int)TopRight.X &&
                           (int)obj.BottomLeft.X == (int)BottomRight.X
                           )
                        {
                            XAxis.Add((int)TopRight.X);
                        }
                        // right edge of selected
                        if ((int)obj.TopRight.X == (int)TopRight.X &&
                           (int)obj.TopRight.X == (int)BottomRight.X &&
                           (int)obj.BottomRight.X == (int)TopRight.X &&
                           (int)obj.BottomRight.X == (int)BottomRight.X
                           )
                        {
                            XAxis.Add((int)TopRight.X);
                        }

                        // left edge of selected
                        if ((int)obj.TopLeft.X == (int)TopLeft.X &&
                           (int)obj.TopLeft.X == (int)BottomLeft.X &&
                           (int)obj.BottomLeft.X == (int)TopLeft.X &&
                           (int)obj.BottomLeft.X == (int)BottomLeft.X
                           )
                        {
                            XAxis.Add((int)TopLeft.X);
                        }
                        if ((int)obj.TopRight.X == (int)TopLeft.X &&
                           (int)obj.TopRight.X == (int)BottomLeft.X &&
                           (int)obj.BottomRight.X == (int)TopLeft.X &&
                           (int)obj.BottomRight.X == (int)BottomLeft.X
                           )
                        {
                            XAxis.Add((int)TopLeft.X);
                        }


                        // top edge of selected
                        if ((int)obj.TopLeft.Y == (int)TopLeft.Y &&
                          (int)obj.TopLeft.Y == (int)TopRight.Y &&
                          (int)obj.TopRight.Y == (int)TopLeft.Y &&
                          (int)obj.TopRight.Y == (int)TopRight.Y
                          )
                        {
                            YAxis.Add((int)TopLeft.Y);
                        }
                        if ((int)obj.BottomLeft.Y == (int)TopLeft.Y &&
                           (int)obj.BottomLeft.Y == (int)TopRight.Y &&
                           (int)obj.BottomRight.Y == (int)TopLeft.Y &&
                           (int)obj.BottomRight.Y == (int)TopRight.Y
                           )
                        {
                            YAxis.Add((int)TopLeft.Y);
                        }

                        // bottom edge of selected
                        if ((int)obj.TopLeft.Y == (int)BottomLeft.Y &&
                          (int)obj.TopLeft.Y == (int)BottomRight.Y &&
                          (int)obj.TopRight.Y == (int)BottomLeft.Y &&
                          (int)obj.TopRight.Y == (int)BottomRight.Y
                          )
                        {
                            YAxis.Add((int)TopLeft.Y);
                        }
                        if ((int)obj.BottomLeft.Y == (int)BottomLeft.Y &&
                           (int)obj.BottomLeft.Y == (int)BottomRight.Y &&
                           (int)obj.BottomRight.Y == (int)BottomLeft.Y &&
                           (int)obj.BottomRight.Y == (int)BottomRight.Y
                           )
                        {
                            YAxis.Add((int)TopLeft.Y);
                        }
                    }


                }
            }
        }



    }
}
