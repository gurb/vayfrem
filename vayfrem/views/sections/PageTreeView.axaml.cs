using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Platform;
using vayfrem.models;
using vayfrem.models.enums;
using vayfrem.models.objects;
using vayfrem.models.objects.@base;
using vayfrem.viewmodels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace vayfrem.views.sections;

public partial class PageTreeView : UserControl
{
    PageTreeViewModel ViewModel { get; set; }

    ImageBrush caretOn;
    ImageBrush caretOff;
    Avalonia.Media.Imaging.Bitmap text;
    Avalonia.Media.Imaging.Bitmap rect;
    Avalonia.Media.Imaging.Bitmap button;

    public PageTreeView()
    {
        ViewModel = App.GetService<PageTreeViewModel>();
        ViewModel.drawPageView += RefreshDraw;
        DataContext = ViewModel;

        InitializeComponent();
        PageMenu.SizeChanged += PageMenu_SizeChanged;
        DeleteNodeButton.Click += DeleteNodeButton_Click; ;

        Init();
        setStyle();
    }

    private void DeleteNodeButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        ViewModel.DeleteNode();
        DrawCanvas();
    }

    private void Init()
    {
        setImages();
    }

    private void setStyle()
    {
        pageMenuHeader.Background = new SolidColorBrush(Avalonia.Media.Color.FromArgb(255, 204, 208, 209));
        pageMenuFooter.Background = new SolidColorBrush(Avalonia.Media.Color.FromArgb(255, 204, 208, 209));
    }

    private void PageMenu_SizeChanged(object? sender, SizeChangedEventArgs e)
    {
        DrawCanvas();
    }


    private void RefreshDraw()
    {
        DrawCanvas();
    }


    private int DrawCanvas(ObservableCollection<GObject>? nodesParam = null, int counterParam = 0)
    {
        int counter;
        ObservableCollection<GObject> nodes = new ObservableCollection<GObject>();
        if (nodesParam == null)
        {
            nodes = ViewModel.Nodes;
            counter = 0;

            //foreach (var node in nodes)
            //{
            //    node.IsDrew = false;
            //}

            PageMenu.Children.Clear();
            PageMenu.Height = ViewModel.Nodes.Count * 26;
        }
        else
        {
            nodes = nodesParam;
            counter = counterParam;
        }


        foreach (var node in nodes)
        {
            Grid? button = MenuButton(node);

            if (button != null)
            {
                Canvas.SetLeft(button, parentCount(node) * 20);
                Canvas.SetTop(button, counter * 26);
                node.ConnectedMenuControl = button;
                button.PointerPressed += Button_PointerPressed;

                PageMenu.Children.Add(button);

                if (node == ViewModel.SelectedObject)
                {
                    Avalonia.Controls.Shapes.Rectangle hover = new Avalonia.Controls.Shapes.Rectangle();
                    hover.Fill = new SolidColorBrush(Avalonia.Media.Color.FromArgb(100, 100, 100, 100));
                    hover.Stroke = Avalonia.Media.Brushes.Gray;
                    hover.StrokeThickness = 2;
                    hover.Width = PageMenu.Bounds.Width;
                    hover.Height = 26;
                    hover.IsEnabled = false;
                    Canvas.SetLeft(hover, 0);
                    Canvas.SetTop(hover, counter * 26);
                    PageMenu.Children.Add(hover);
                }

                counter++;

                if (node.ObjectType == ObjectType.Canvas && node.IsVisible)
                {
                    CanvasObj canvasObj = (CanvasObj)node;

                    ObservableCollection<GObject> childrenNode = new ObservableCollection<GObject>(canvasObj.Children);

                    counter = DrawCanvas(childrenNode, counter);

                }

                node.IsDrew = true;
            }
        }

        return counter;
    }


    private Grid? MenuButton(GObject? node)
    {
        Grid grid = new Grid();

        try
        {
            grid.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch;
            grid.VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center;
            grid.Height = 25;
            grid.Background = Avalonia.Media.Brushes.White;
            grid.Width = PageMenu.Bounds.Width;
            grid.ColumnDefinitions = new ColumnDefinitions("25, 25, *");


            if (node.ObjectType == ObjectType.Canvas)
            {
                Avalonia.Controls.Shapes.Rectangle caret = new Avalonia.Controls.Shapes.Rectangle();
                caret.Width = 16;
                caret.Height = 16;
                caret.PointerPressed += Caret_PointerPressed;
                node.CaretControl = caret;
                if (node.IsVisible)
                {
                    caret.Fill = caretOn;
                }
                else
                {
                    caret.Fill = caretOff;
                }

                Grid.SetColumn(caret, 0);
                grid.Children.Add(caret);
            }

            Avalonia.Controls.Image icon = new Avalonia.Controls.Image();
            icon.Width = 20;
            icon.Height = 20;
            if (node.ObjectType == ObjectType.Text)
            {
                icon.Source = text;
            }
            if (node.ObjectType == ObjectType.Canvas)
            {
                icon.Source = rect;
            }
            if (node.ObjectType == ObjectType.Button)
            {
                icon.Source = button;
            }
            Grid.SetColumn(icon, 1);
            grid.Children.Add(icon);

            Label textBlock = new Label();
            textBlock.Content = node.ObjectType.ToString();
            textBlock.Height = 20;
            textBlock.FontSize = 12;
            textBlock.Background = Avalonia.Media.Brushes.Transparent;
            textBlock.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch;
            textBlock.VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center;

            Grid.SetColumn(textBlock, 2);
            grid.Children.Add(textBlock);
        }
        catch (Exception ex) { }


        return grid;
    }

    private void Caret_PointerPressed(object? sender, Avalonia.Input.PointerPressedEventArgs e)
    {
        var point = e.GetCurrentPoint(sender as Control);

        if (point.Properties.IsLeftButtonPressed)
        {
            if (sender == null)
                return;

            var node = getNodeCaret((Control)sender);

            if (node != null)
            {
                node.IsVisible = !node.IsVisible;

                DrawCanvas();
            }
        }
    }


    private List<GObject> OneList (ObservableCollection<GObject> nodes)
    {
        List<GObject> result = new List<GObject>();

        foreach (var node in nodes) 
        { 
            result.Add(node);

            if(node.ObjectType == ObjectType.Canvas)
            {
                CanvasObj canvasObj = (CanvasObj)node;

                var list = OneList(new ObservableCollection<GObject>(canvasObj.Children));

                result.AddRange(list);
            }
        }

        return result;
    }

    private GObject? getNodeCaret(Control control)
    {
        List<GObject> nodes = OneList(ViewModel.Nodes);

        return nodes.FirstOrDefault(x => x.CaretControl == control);
    }

    private GObject? getNodeButtonMenu(Control control)
    {
        List<GObject> nodes = OneList(ViewModel.Nodes);

        return nodes.FirstOrDefault(x => x.ConnectedMenuControl == control);
    }


    private void iterateVisibleNode(GObject node)
    {
        CanvasObj canvasObj = (CanvasObj)node;
        ObservableCollection<GObject> childrenNode = new ObservableCollection<GObject>(canvasObj.Children);
        
        foreach (var child in childrenNode)
        {
            child.IsDrew = true;

            iterateVisibleNode(child);
        }
    }


    private int parentCount(GObject node, int counter = 0)
    {
        int parentCounter = counter;

        if (node.Parent != null)
        {
            parentCounter++;
            return parentCount((GObject)node.Parent, parentCounter);
        }

        return parentCounter;

    }

    private void Button_PointerPressed(object? sender, Avalonia.Input.PointerPressedEventArgs e)
    {
        var point = e.GetCurrentPoint(sender as Control);

        if(point.Properties.IsLeftButtonPressed)
        {
            if (sender == null)
                return;

            var node = getNodeButtonMenu((Control)sender);

            if(node != null)
            {
                ViewModel.SetSelectedObject(node);
                ViewModel.SetSelected(node);
                DrawCanvas();
            }
        }
    }

    private void setImages()
    {
        caretOn = new ImageBrush(new Avalonia.Media.Imaging.Bitmap(AssetLoader.Open(new Uri("avares://vayfrem/assets/careton.png"))));
        caretOff = new ImageBrush(new Avalonia.Media.Imaging.Bitmap(AssetLoader.Open(new Uri("avares://vayfrem/assets/caret.png"))));
        text = new Avalonia.Media.Imaging.Bitmap(AssetLoader.Open(new Uri("avares://vayfrem/assets/text.png")));
        rect = new Avalonia.Media.Imaging.Bitmap(AssetLoader.Open(new Uri("avares://vayfrem/assets/rect.png")));
        button = new Avalonia.Media.Imaging.Bitmap(AssetLoader.Open(new Uri("avares://vayfrem/assets/button.png")));
    }
}