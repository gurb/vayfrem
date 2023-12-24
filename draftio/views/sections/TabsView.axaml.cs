using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Platform;
using draftio.models;
using draftio.viewmodels;
using System;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Xml.Linq;

namespace draftio;

public partial class TabsView : UserControl
{
    TabViewModel ViewModel { get; set; }
    ProjectTreeViewModel projectViewModel { get; set; }

    LinearGradientBrush tabButtonBgGradient;
    SolidColorBrush tabButtonBg;


    public TabsView()
    {
        ViewModel = App.GetService<TabViewModel>();
        projectViewModel = App.GetService<ProjectTreeViewModel>();

        ViewModel.drawDelegate += DrawCanvas;
        DataContext = ViewModel;

        InitializeComponent();

        SetStyle();
    }

    private void SetStyle()
    {
        TabMenu.Background = new SolidColorBrush(Avalonia.Media.Color.FromArgb(255, 237, 236, 235));

        SetColors();
    }

    private void SetColors ()
    {
        tabButtonBgGradient = new LinearGradientBrush
        {
            GradientStops = new GradientStops
                {
                    new GradientStop(Color.FromRgb(237, 236, 234), 0),  
                    new GradientStop(Color.FromRgb(225, 225, 220), 1)   
                },
            StartPoint = new RelativePoint(0, 0, RelativeUnit.Relative),  
            EndPoint = new RelativePoint(0, 1, RelativeUnit.Relative)    
        };

        tabButtonBg = new SolidColorBrush(Avalonia.Media.Color.FromArgb(255, 225, 225, 220));
    }

    private void DrawCanvas()
    {
        int counter = 0;

        TabMenu.Children.Clear();

        double tabMenuWidth = CalcTabMenuWidth();
        if(TabMenuScrollParent.Bounds.Width < tabMenuWidth)
        {
            TabMenu.Width = tabMenuWidth;
        } 
        else
        {
            TabMenu.Width = TabMenuScrollParent.Bounds.Width;
        }


        foreach (var node in ViewModel.Nodes)
        {

            Border? button = TabButton(node);

            Canvas.SetLeft(button, (counter * button.Width) + counter * 4);
            Canvas.SetTop(button, 5);
            node.ConnectedTabControl = button;
            button.PointerPressed += Button_PointerPressed;


            if(ViewModel.SelectedNode == node)
            {
                //button.Background = new SolidColorBrush(Avalonia.Media.Color.FromArgb(255, 100, 100, 100));
                button.BorderBrush = Brushes.Gray;
                button.BorderThickness = new Avalonia.Thickness(2, 2, 2, 0);
            }

            TabMenu.Children.Add(button);

            counter++;
        }
    }

    private void Button_PointerPressed(object? sender, Avalonia.Input.PointerPressedEventArgs e)
    {
        var point = e.GetCurrentPoint(sender as Control);

        if (point.Properties.IsLeftButtonPressed)
        {
            var node = ViewModel.Nodes.Where(x => x.ConnectedTabControl == sender as Control).FirstOrDefault();

            if (node != null)
            {
                ViewModel.SetSelected(node);
                DrawCanvas();
            }
        }
    }

    private double CalcTabMenuWidth()
    {
        double width = 0;
        foreach (var node in ViewModel.Nodes)
        {
            width += node.Name.Length * 10 + 50;
            width += 4; 
        }

        return width;
    }

    private Border TabButton(Node node)
    {
        Border border = new Border();
        border.CornerRadius = new Avalonia.CornerRadius(1);
        border.BorderBrush = new SolidColorBrush(Avalonia.Media.Color.FromArgb(255, 200, 200, 200));
        border.BorderThickness = new Avalonia.Thickness(1, 1, 1, 0);
        border.Width = node.Name.Length * 10 + 50;

        Grid grid = new Grid();
        grid.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch;
        grid.VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center;
        grid.Height = 25;
        grid.Background = (ViewModel.SelectedNode != null && ViewModel.SelectedNode == node) ? Brushes.White  : tabButtonBg;
        grid.Width = node.Name.Length * 10 + 50;
        string textWidth = ((node.Name.Length * 10) + 25).ToString();
        grid.ColumnDefinitions = new ColumnDefinitions($"{textWidth}, 30");


        Label textBlock = new Label();
        if (node.IsSaved)
            textBlock.Content = node.Name;
        else
            textBlock.Content = node.Name + "*";
        textBlock.Height = 20;
        textBlock.FontSize = (ViewModel.SelectedNode != null && ViewModel.SelectedNode == node) ? 12 : 12;
        textBlock.FontWeight = (ViewModel.SelectedNode != null && ViewModel.SelectedNode == node) ? FontWeight.Bold : FontWeight.Normal;
        textBlock.Margin = new Avalonia.Thickness(10, 0,0,0);
        textBlock.Background = Avalonia.Media.Brushes.Transparent;
        textBlock.Foreground = (ViewModel.SelectedNode != null && ViewModel.SelectedNode == node) ? Brushes.Black : new SolidColorBrush(Avalonia.Media.Color.FromArgb(255, 75, 75, 75));
        textBlock.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch;
        textBlock.VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center;
        Grid.SetColumn(textBlock, 0);
        grid.Children.Add(textBlock);


        // close button
        Avalonia.Controls.Shapes.Rectangle close = new Avalonia.Controls.Shapes.Rectangle();
        close.Width = 10;
        close.Height = 10;
        close.PointerPressed += Close_PointerPressed; ;
        node.CloseControl = close;
        close.Fill = new ImageBrush(new Avalonia.Media.Imaging.Bitmap(AssetLoader.Open(new Uri("avares://draftio/assets/close.png"))));
        Grid.SetColumn(close, 1);
        grid.Children.Add(close);

        border.Child = grid;

        return border;
    }

    private void Close_PointerPressed(object? sender, Avalonia.Input.PointerPressedEventArgs e)
    {
        var point = e.GetCurrentPoint(sender as Control);

        if (point.Properties.IsLeftButtonPressed)
        {
            var node = ViewModel.Nodes.Where(x => x.CloseControl == sender as Control).FirstOrDefault();

            if (node != null)
            {
                ViewModel.RemoveNode(node);
                DrawCanvas();
            }
        }
    }
}