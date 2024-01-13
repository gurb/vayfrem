using Avalonia.Controls;
using Avalonia.Media;
using draftio.models.structs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace draftio.views.components
{
    public class DataGrid: Grid
    {
        ScrollViewer scrollViewer;
        public List<Property>? Properties { get; set; }

        Grid HeaderGrid;
        Grid MainGrid;
        StackPanel hcol1;
        StackPanel hcol2;
        StackPanel col1;
        StackPanel col2;

        GridSplitter headerSplitter;
        GridSplitter mainSplitter;

        public DataGrid()
        {
            scrollViewer = new ScrollViewer();
            scrollViewer.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch;

            HeaderGrid = new Grid();
            HeaderGrid.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch;
            HeaderGrid.SizeChanged += HeaderGrid_SizeChanged;
            HeaderGrid.Height = 30;

            MainGrid = new Grid();
            MainGrid.Background = Brushes.White;
            MainGrid.SizeChanged += MainGrid_SizeChanged;
            MainGrid.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch;
            MainGrid.VerticalAlignment = Avalonia.Layout.VerticalAlignment.Top;
            //MainGrid.VerticalAlignment = Avalonia.Layout.VerticalAlignment.Stretch;

            HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch;
            VerticalAlignment = Avalonia.Layout.VerticalAlignment.Stretch;
            Background = Brushes.White;

            RowDefinitions = new RowDefinitions("30, *");
            HeaderGrid.ColumnDefinitions = new ColumnDefinitions("*,1,*");
            MainGrid.ColumnDefinitions = new ColumnDefinitions("*,1,*");

            mainSplitter = new GridSplitter();
            mainSplitter.DragDelta += MainSplitter_DragDelta;
            headerSplitter = new GridSplitter();
            headerSplitter.DragDelta += HeaderSplitter_DragDelta;

            SetHeaderGrid();
            SetMainGrid();
            scrollViewer.Content = MainGrid;

            Grid.SetRow(HeaderGrid, 0);
            this.Children.Add(HeaderGrid);

            Grid.SetRow(scrollViewer, 1);
            this.Children.Add(scrollViewer);
        }

        private void MainGrid_SizeChanged(object? sender, SizeChangedEventArgs e)
        {
            //HeaderGrid.ColumnDefinitions[0].Width = new GridLength(MainGrid.ColumnDefinitions[0].ActualWidth, GridUnitType.Pixel);
            var sum = hcol1.Bounds.Width + hcol2.Bounds.Width;
            if (sum <= 0)
                return;


            HeaderGrid.ColumnDefinitions[0].Width = new GridLength(MainGrid.ColumnDefinitions[0].ActualWidth / sum, GridUnitType.Star);
            HeaderGrid.ColumnDefinitions[2].Width = new GridLength(MainGrid.ColumnDefinitions[2].ActualWidth / sum, GridUnitType.Star);
        }

        private void HeaderGrid_SizeChanged(object? sender, SizeChangedEventArgs e)
        {
            var sum = col1.Bounds.Width + col2.Bounds.Width;
            if (sum <= 0)
                return;

            MainGrid.ColumnDefinitions[0].Width = new GridLength(HeaderGrid.ColumnDefinitions[0].ActualWidth / sum, GridUnitType.Star);
            MainGrid.ColumnDefinitions[2].Width = new GridLength(HeaderGrid.ColumnDefinitions[2].ActualWidth / sum, GridUnitType.Star);
        }

        private void HeaderSplitter_DragDelta(object? sender, Avalonia.Input.VectorEventArgs e)
        {
            MainGrid.ColumnDefinitions[0].Width = new GridLength(HeaderGrid.ColumnDefinitions[0].ActualWidth, GridUnitType.Pixel);
            MainGrid.ColumnDefinitions[2].Width = new GridLength(HeaderGrid.ColumnDefinitions[2].ActualWidth, GridUnitType.Pixel);
        }

        private void MainSplitter_DragDelta(object? sender, Avalonia.Input.VectorEventArgs e)
        {
            HeaderGrid.ColumnDefinitions[0].Width = new GridLength(MainGrid.ColumnDefinitions[0].ActualWidth, GridUnitType.Pixel);
            HeaderGrid.ColumnDefinitions[2].Width = new GridLength(MainGrid.ColumnDefinitions[2].ActualWidth, GridUnitType.Pixel);
        }

        private void SetHeaderGrid()
        {
            hcol1 = new StackPanel();
            hcol1.Orientation = Avalonia.Layout.Orientation.Vertical;

            TextBlock headerCol1 = new TextBlock();
            headerCol1.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch;
            headerCol1.Height = 30;
            headerCol1.Text = "Property";
            headerCol1.Background = Brushes.LightGray;
            headerCol1.Padding = new Avalonia.Thickness(5);

            hcol1.Children.Add(headerCol1);

            hcol2 = new StackPanel();
            hcol2.Orientation = Avalonia.Layout.Orientation.Vertical;

            TextBlock headerCol2 = new TextBlock();
            headerCol2.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch;
            headerCol2.Height = 30;
            headerCol2.Text = "Value";
            headerCol2.Background = Brushes.LightGray;
            headerCol2.Padding = new Avalonia.Thickness(5);

            hcol2.Children.Add(headerCol2);

            headerSplitter.Background = Brushes.LightGray;
            headerSplitter.Width = 1;
            headerSplitter.BorderThickness = new Avalonia.Thickness(0);
            headerSplitter.MaxWidth = 1;
            headerSplitter.MinWidth = 1;

            Grid.SetColumn(headerSplitter, 1);
            this.HeaderGrid.Children.Add(headerSplitter);

            Grid.SetColumn(hcol1, 0);
            this.HeaderGrid.Children.Add(hcol1);

            Grid.SetColumn(hcol2, 2);
            this.HeaderGrid.Children.Add(hcol2);
        }

        private void SetMainGrid()
        {
            col1 = new StackPanel();
            col1.Orientation = Avalonia.Layout.Orientation.Vertical;

            col2 = new StackPanel();
            col2.Orientation = Avalonia.Layout.Orientation.Vertical;

            mainSplitter.Background = Brushes.LightGray;
            mainSplitter.Width = 1;
            mainSplitter.BorderThickness = new Avalonia.Thickness(0);
            mainSplitter.MaxWidth = 1;
            mainSplitter.MinWidth = 1;

            Grid.SetColumn(mainSplitter, 1);
            this.MainGrid.Children.Add(mainSplitter);

            Grid.SetColumn(col1, 0);
            this.MainGrid.Children.Add(col1);

            Grid.SetColumn(col2, 2);
            this.MainGrid.Children.Add(col2);
        }


        public void SetProperties(List<Property>? properties)
        {
            this.MainGrid.Children.Clear();
            col1.Children.Clear();
            col2.Children.Clear();
            this.MainGrid.Height = properties.Count * 30 + properties.Count * 1;
            SetMainGrid();

            this.Properties = properties;
            foreach (var property in Properties!)
            {
                col1.Children.Add(PropertyTextBlock(property.Name));
                col1.Children.Add(GridSeparator());

                property.Control.Height = 30;
                property.Control.MinHeight = 30;
                col2.Children.Add(property.Control);
                col2.Children.Add(GridSeparator());
            }
        }

        private Separator GridSeparator()
        {
            Separator separator = new Separator();

            separator.Height = 1;
            separator.Background = Brushes.LightGray;
            separator.Margin = new Avalonia.Thickness(0);

            return separator;
        }


        private TextBlock PropertyTextBlock(string? text)
        {
            TextBlock propertyTextBlock = new TextBlock();
            propertyTextBlock.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch;
            propertyTextBlock.Height = 30;
            propertyTextBlock.Text = text;
            propertyTextBlock.Background = Brushes.White;
            propertyTextBlock.Padding = new Avalonia.Thickness(5);

            return propertyTextBlock;
        }

    }
}
