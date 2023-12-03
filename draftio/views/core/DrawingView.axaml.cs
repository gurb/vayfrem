using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using draftio.models.dtos;
using draftio.services;
using draftio.viewmodels;
using System;

namespace draftio;

public partial class DrawingView : UserControl
{
    public DrawingViewModel ViewModel { get; private set; }
    private readonly RenderManager renderManager;

    // status of draw operation
    private bool isActive;
    private Point currentPosition;
    private Point firstPosition;
    private Point lastPosition;
    
    public DrawingView()
    {
        ViewModel = App.GetService<DrawingViewModel>();
        DataContext = ViewModel;

        renderManager = App.GetService<RenderManager>();

        InitializeComponent();
        Display.PointerPressed += OnPointerPressed;
        Display.PointerReleased += OnPointerReleased;
        Display.PointerMoved += OnPointerMoved;
    }

    private void OnPointerMoved(object? sender, PointerEventArgs e)
    {
        currentPosition = e.GetPosition(this);
    }

    private void OnPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        var point = e.GetCurrentPoint(sender as Control);


        if (isActive)
        {
            isActive = false;
            lastPosition = point.Position;

            handle();
            draw();
        }
    }

    private void OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        var point = e.GetCurrentPoint(sender as Control);

        var position = point.Position;

        if (point.Properties.IsLeftButtonPressed)
        {
            isActive = true;
            firstPosition = position;
        } 
    }

    private void handle()
    {
        // just draw rectangle for now

        // canvas can be used as rendertarget

        PassData passData = new PassData
        {
            X = Math.Min(firstPosition.X, lastPosition.X),
            Y = Math.Min(firstPosition.Y, lastPosition.Y),
            Width = Math.Abs(firstPosition.X - lastPosition.X),
            Height = Math.Abs(firstPosition.Y - lastPosition.Y)
        };

        ViewModel.AddObject(passData);
    }
    private void draw()
    {
        Display.Children.Clear();

        renderManager.Render(Display, ViewModel.Objects);
    }
}