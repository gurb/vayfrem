using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Markup.Xaml;
using System;
using draftio.viewmodels;
using System.Diagnostics;

namespace draftio;

public partial class LayoutView : UserControl
{
    LayoutViewModel ViewModel;

    Border? ghostItem;

    Flyout flyout = new Flyout();

    private Point _ghostPosition = new(0, 0);
    private readonly Point _mouseOffset = new(-5, -5);

    int counter = 0;


    public LayoutView()
    {
        ViewModel = App.GetService<LayoutViewModel>();
        DataContext = ViewModel;

        InitializeComponent();

        OverlayLayout.Background = new SolidColorBrush(Avalonia.Media.Color.FromArgb(0, 255, 255, 255));



        this.PointerMoved += LayoutView_PointerMoved;
        this.PointerReleased += LayoutView_PointerReleased;

        SetGhostItem();
    }

    private void LayoutView_PointerReleased(object? sender, Avalonia.Input.PointerReleasedEventArgs e)
    {
        if (ViewModel.IsDrag)
        {
            ViewModel.Counter++;

            ViewModel.IsDrag = false;
            ghostItem.IsVisible = false;
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
        ghostItem.IsVisible = true;

        Canvas.SetLeft(ghostItem, _ghostPosition.X);
        Canvas.SetTop(ghostItem,_ghostPosition.Y);
    }


    private void SetGhostItem()
    {
        ghostItem = new Border();
        ghostItem.BorderThickness = new Thickness(1);
        ghostItem.BorderBrush = Brushes.Black;
        ghostItem.Background = Brushes.White;
        ghostItem.CornerRadius = new CornerRadius(2);
        //ghostItem.Padding = new Thickness(10, 10, 10, 10);
        //ghostItem.Margin = new Thickness(5, 5, 5, 5);
        ghostItem.IsVisible = false;

        TextBlock text = new TextBlock();
        text.Height = 50;
        text.Width = 200;

        text.Text = "Button";
        text.Background = Brushes.White;

        ghostItem.Child = text;

        Canvas.SetLeft(ghostItem, _ghostPosition.X);
        Canvas.SetTop(ghostItem, _ghostPosition.Y);

        OverlayLayout.Children.Add(ghostItem);

    }
}