using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Media;
using Avalonia.Styling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace draftio.views.components
{
    public class SliderWithValue: Grid
    {

        Slider slider;
        TextBlock textBlock;

        public delegate void ValueChangeDelegate();
        public ValueChangeDelegate? ValueChanged;

        public SliderWithValue() 
        {

            Style style = new Style(x => x.OfType<Slider>())
            {
                Setters =
                {
                    new Setter(Slider.MinHeightProperty, 20.0),
                    new Setter(Slider.HeightProperty, 20.0),
                    new Setter(Slider.MaxHeightProperty, 20.0),
                }
            };


            ColumnDefinitions = new ColumnDefinitions("*, 50");

            slider = new Slider();
            //slider.ValueChanged += Slider_ValueChanged;
            //slider.Padding = new Thickness(0);
            //slider.Margin = new Thickness(5, 0, 10, 0);
            //slider.TickPlacement = TickPlacement.None;
            slider.Styles.Add(style);

            slider.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch;
            slider.VerticalAlignment = Avalonia.Layout.VerticalAlignment.Top;

            textBlock = new TextBlock();
            textBlock.Padding = new Thickness(0);
            textBlock.Margin = new Thickness(0);
            textBlock.MinHeight = 30;
            textBlock.Height = 30;
            textBlock.VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center;

            Grid.SetColumn(slider, 0);
            Children.Add(slider);   
            Grid.SetColumn(textBlock, 1);
            Children.Add(textBlock);
        }

        private void Slider_ValueChanged(object? sender, Avalonia.Controls.Primitives.RangeBaseValueChangedEventArgs e)
        {
            if(ValueChanged != null)
            {
                ValueChanged.Invoke();
            }
        }
    }
}
