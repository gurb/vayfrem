using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using vayfrem.models.objects.@base;
using vayfrem.models.objects.components;
using vayfrem.viewmodels;
using System;

namespace vayfrem.views.sections;

public partial class ComponentView : UserControl
{
    ComponentViewModel ViewModel { get; set; }
    LayoutViewModel LayoutViewModel { get; set; }   
    ScrollViewer scrollViewer;
    StackPanel stackPanel;

    Border button;

    Border? ghostItem;

    Flyout flyout = new Flyout();

    private bool isPressed = false;

    private Point _ghostPosition = new(0, 0);
    private readonly Point _mouseOffset = new(-5, -5);


    public ComponentView()
    {
        ViewModel = App.GetService<ComponentViewModel>();
        LayoutViewModel = App.GetService<LayoutViewModel>();
        DataContext = ViewModel;



        InitializeComponent();

        

        Init();
        setStyle();


    }

    private void setStyle()
    {
        componentsMenuHeader.Background = new SolidColorBrush(Avalonia.Media.Color.FromArgb(255, 204, 208, 209));
        componentsMenuFooter.Background = new SolidColorBrush(Avalonia.Media.Color.FromArgb(255, 204, 208, 209));
    }

  
    private void Init()
    {
        SetComponents();
    }

    private void SetComponents()
    {
        button = new Border();
        button.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch;
        button.BorderThickness = new Thickness(1);
        button.BorderBrush = Brushes.Black;
        button.Background = Brushes.White;
        button.CornerRadius = new CornerRadius(2);
        button.Padding = new Thickness(10, 10, 10, 10);
        button.Margin = new Thickness(5, 5, 5, 5);
        button.PointerPressed += Button_PointerPressed;

        TextBlock text = new TextBlock();
        text.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch;
        text.Height = 18;
        text.Text = "Button";
        text.Background = Brushes.White;

        button.Child = text;

        ComponentMenu.Children.Add(button);
    }

    private async void Button_PointerPressed(object? sender, Avalonia.Input.PointerPressedEventArgs e)
    {
        // drag started
        Border dragButton = sender as Border;

        isPressed = true;
        LayoutViewModel.IsDrag = true;
        LayoutViewModel.DragObject = new ButtonObj();
        LayoutViewModel.IsDragCompleted = false;
        LayoutViewModel.Counter = 1;
    }
}


