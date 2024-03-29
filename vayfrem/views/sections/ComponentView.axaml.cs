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
    Border image;

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
        ComponentMenu.Children.Add(GetComponent("Button", "button"));
        ComponentMenu.Children.Add(GetComponent("Image", "image"));
        ComponentMenu.Children.Add(GetComponent("Container", "container"));
        ComponentMenu.Children.Add(GetComponent("Container Fluid", "containerfluid"));
        ComponentMenu.Children.Add(GetComponent("Relative Container", "relativecontainer"));
        ComponentMenu.Children.Add(GetComponent("Row", "row"));
        ComponentMenu.Children.Add(GetComponent("Column", "column"));
    }

    private Border GetComponent(string Text, string name)
    {
        Border border = new Border();
        border.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch;
        border.BorderThickness = new Thickness(0);
        border.BorderBrush = Brushes.Black;
        border.Background = new SolidColorBrush(Avalonia.Media.Color.FromArgb(255, 230, 230, 230));
        border.CornerRadius = new CornerRadius(5);
        border.Padding = new Thickness(10, 10, 10, 10);
        border.Margin = new Thickness(5, 5, 5, 5);
        border.PointerPressed += Drag_PointerPressed;
        border.Name = name;

        TextBlock text = new TextBlock();
        text.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch;
        text.VerticalAlignment = Avalonia.Layout.VerticalAlignment.Stretch;
        text.Height = 18;
        text.Text = Text;
        text.Background = Brushes.Transparent;

        border.Child = text;

        return border;
    }

    private async void Drag_PointerPressed(object? sender, Avalonia.Input.PointerPressedEventArgs e)
    {
        // drag started
        Border dragButton = sender as Border;
    
        isPressed = true;
        LayoutViewModel.IsDrag = true;
        if(dragButton!.Name == "button") 
        {
            LayoutViewModel.DragObject = new ButtonObj();
        }
        else if(dragButton.Name == "image") 
        {
            LayoutViewModel.DragObject = new ImageObj();
        }
        else if (dragButton.Name == "column")
        {
            LayoutViewModel.DragObject = new ColumnObj();
        }
        else if (dragButton.Name == "row")
        {
            LayoutViewModel.DragObject = new RowObj();
        }
        else if (dragButton.Name == "container")
        {
            LayoutViewModel.DragObject = new ContainerObj();
        }
        else if (dragButton.Name == "containerfluid")
        {
            LayoutViewModel.DragObject = new ContainerFluidObj();
        }
        else if (dragButton.Name == "relativecontainer")
        {
            LayoutViewModel.DragObject = new RelativeContainerObj();
        }

        LayoutViewModel.IsDragCompleted = false;
        LayoutViewModel.Counter = 1;
    }
}