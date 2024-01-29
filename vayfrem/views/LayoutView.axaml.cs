using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Markup.Xaml;
using System;
using vayfrem.viewmodels;
using System.Diagnostics;
using System.Collections.Generic;
using vayfrem.services;
using Avalonia.Svg;
using vayfrem.models.objects.components;

namespace vayfrem;

public partial class LayoutView : UserControl
{
    LayoutViewModel ViewModel;
    private readonly ObjectMenuManager objectMenuManager;

    Border? ghostItem;
    Avalonia.Svg.Svg ghostSvg;

    private Point _ghostPosition = new(0, 0);
    private readonly Point _mouseOffset = new(-5, -5);

    int counter = 0;

    Grid objectMenuGrid;
    ContextMenu objectMenu;
    MenuItem copyMenuItem;
    MenuItem pasteMenuItem;

    public LayoutView()
    {
        ViewModel = App.GetService<LayoutViewModel>();
        DataContext = ViewModel;

        objectMenuManager = App.GetService<ObjectMenuManager>();

        InitializeComponent();

        OverlayLayout.Background = new SolidColorBrush(Avalonia.Media.Color.FromArgb(0, 255, 255, 255));
        OverlayLayout.IsEnabled = false;


        this.PointerMoved += LayoutView_PointerMoved;
        this.PointerReleased += LayoutView_PointerReleased;
        this.PointerPressed += LayoutView_PointerPressed;

        SetGhostItem();
        SetObjectMenu();
    }

    private void LayoutView_PointerPressed(object? sender, Avalonia.Input.PointerPressedEventArgs e)
    {
        if (ViewModel.IsOpenMenu)
        {
            ViewModel.IsOpenMenu = false;
            CloseObjectMenu();
        }

        if(ViewModel.DragObject != null && ViewModel.DragObject.ObjectType == models.enums.ObjectType.Svg)
        {
            SvgObj svgObj = (SvgObj)ViewModel.DragObject;

            ghostSvg.Path = svgObj.Path;
        }
    }

    private void LayoutView_PointerReleased(object? sender, Avalonia.Input.PointerReleasedEventArgs e)
    {
        var point = e.GetPosition(sender as Control);

        if (ViewModel.IsDrag)
        {
            ViewModel.Counter++;

            ViewModel.IsDrag = false;

            ghostItem.IsVisible = false;
            ghostSvg.IsVisible = false;
        }

        if(ViewModel.IsOpenMenu)
        {
            OpenObjectMenu(point);
        }
    }


    private void LayoutView_PointerMoved(object? sender, Avalonia.Input.PointerEventArgs e)
    {
        var point = e.GetPosition(sender as Control);
        _ghostPosition = new Avalonia.Point((int)point.X, (int)point.Y);

        if (ViewModel.IsDrag)
        {
            DrawDrag();
        }
        else
        {
            ViewModel.DragObject = null;
            ViewModel.IsDragCompleted = true;
        }
    }

    public void DrawDrag()
    {
        if(ViewModel.DragObject.ObjectType == models.enums.ObjectType.Button)
        {
            ghostItem.IsVisible = true;

            Canvas.SetLeft(ghostItem, _ghostPosition.X - (ghostItem.Bounds.Width / 2));
            Canvas.SetTop(ghostItem, _ghostPosition.Y - (ghostItem.Bounds.Height / 2));
        }
        if (ViewModel.DragObject.ObjectType == models.enums.ObjectType.Svg)
        {
            ghostSvg.IsVisible = true;

            Canvas.SetLeft(ghostSvg, _ghostPosition.X - (ghostSvg.Width / 2));
            Canvas.SetTop(ghostSvg, _ghostPosition.Y - (ghostSvg.Height / 2));
        }
    }

    private void SetObjectMenu()
    {
        objectMenuGrid = new Grid();
        objectMenuGrid.ColumnDefinitions = new ColumnDefinitions("200");
        objectMenuGrid.IsVisible = false;

        objectMenu = new ContextMenu();
        objectMenu.IsEnabled = true;

        Grid.SetColumn(objectMenu, 0);
        objectMenuGrid.Children.Add(objectMenu);

        copyMenuItem = new MenuItem();
        copyMenuItem.Header = "Copy";
        copyMenuItem.PointerPressed += CopyMenuItem_PointerPressed;

        pasteMenuItem = new MenuItem();
        pasteMenuItem.Header = "Paste";
        pasteMenuItem.PointerPressed += PasteMenuItem_PointerPressed;

        objectMenu.ItemsSource = new List<MenuItem>()
        {
            copyMenuItem,
            pasteMenuItem
        };

        OverlayLayout.Children.Add(objectMenuGrid);
    }

    private void PasteMenuItem_PointerPressed(object? sender, Avalonia.Input.PointerPressedEventArgs e)
    {
        objectMenuManager.Paste();
    }

    private void CopyMenuItem_PointerPressed(object? sender, Avalonia.Input.PointerPressedEventArgs e)
    {
        objectMenuManager.Copy();
    }

    private void OpenObjectMenu(Avalonia.Point point)
    {
        OverlayLayout.IsEnabled = true;
        objectMenuGrid.IsVisible = true;
        Canvas.SetLeft(objectMenuGrid, point.X);
        Canvas.SetTop(objectMenuGrid, point.Y);
    }

    private void CloseObjectMenu()
    {
        OverlayLayout.IsEnabled = false;
        objectMenuGrid.IsVisible = false;
        Canvas.SetLeft(objectMenuGrid, -1000);
        Canvas.SetTop(objectMenuGrid, -1000);
    }

    private void SetGhostItem()
    {
        ghostItem = new Border();
        ghostItem.BorderThickness = new Thickness(3);
        ghostItem.BorderBrush = Brushes.Black;
        ghostItem.Background = Brushes.White;
        ghostItem.CornerRadius = new CornerRadius(5);
        ghostItem.Padding = new Thickness(0, 5, 0, 5);
        ghostItem.IsVisible = false;

        TextBlock text = new TextBlock();
        text.Height = 20;
        text.Width = 200;

        text.Text = "Button";
        text.TextAlignment = TextAlignment.Center;
        text.Background = Brushes.White;

        ghostItem.Child = text;

        Canvas.SetLeft(ghostItem, _ghostPosition.X);
        Canvas.SetTop(ghostItem, _ghostPosition.Y);


        Uri uri = new System.Uri("C://test");
        ghostSvg = new Avalonia.Svg.Svg(uri);
        ghostSvg.Width = 64;
        ghostSvg.Height = 64;
        ghostSvg.IsVisible = false;

        OverlayLayout.Children.Add(ghostItem);
        OverlayLayout.Children.Add(ghostSvg);

    }
}