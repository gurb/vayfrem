using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using System.Collections.Generic;

namespace vayfrem.views.sections;

public partial class SvgView : UserControl
{
    List<Canvas> svgNodes = new List<Canvas>();

    public SvgView()
    {
        InitializeComponent();

        SvgMenu.Background = Brushes.Aqua;
        SvgMenu.SizeChanged += SvgMenu_SizeChanged;

        setStyle();

        set();
    }

   

    private void setStyle()
    {
        svgMenuHeader.Background = new SolidColorBrush(Avalonia.Media.Color.FromArgb(255, 204, 208, 209));
        svgMenuFooter.Background = new SolidColorBrush(Avalonia.Media.Color.FromArgb(255, 204, 208, 209));
    }

    private Canvas SvgNode()
    {
        Canvas canvas = new Canvas();

        canvas.Width = 25;
        canvas.Height = 25;

        canvas.Background = Brushes.Gray;

        return canvas;
    }


    private void DrawCanvas()
    {
        SvgMenu.Children.Clear();

        if(SvgMenu.Bounds.Width > 0)
        {
            int columnLen = 0;
            int rowLen = 0;

            if((30 * svgNodes.Count) <= (int)SvgMenu.Bounds.Width)
            {
                columnLen = svgNodes.Count;
                rowLen = 1;
            } 
            else
            {
                columnLen = (int)SvgMenu.Bounds.Width / 30;

                if(columnLen == 0)
                {
                    columnLen = 1;
                }

                rowLen = svgNodes.Count / columnLen;


                if ((svgNodes.Count % columnLen) != 0)
                {
                    rowLen += 1;
                }
            }

            SvgMenu.Height = rowLen * 35;

            int counter = 0;
            for (global::System.Int32 y = 0; y < rowLen; y++)
            {
                for (global::System.Int32 x = 0; x < columnLen; x++)
                {
                    if(counter == svgNodes.Count)
                    {
                        return;
                    }

                    Canvas.SetLeft(svgNodes[counter], (x * 25 + x * 5) + 5);
                    Canvas.SetTop(svgNodes[counter], (y * 25 + y * 5) + 5);
                    SvgMenu.Children.Add(svgNodes[counter]);

                    counter++;
                }
            }
        }
    }

    private void SvgMenu_SizeChanged(object? sender, SizeChangedEventArgs e)
    {
        DrawCanvas();
    }


    private void set()
    {
        for (int i = 0; i < 10; i++)
        {
            svgNodes.Add(SvgNode());
        }
    }

}