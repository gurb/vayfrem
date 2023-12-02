using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using System;

namespace draftio;

public partial class DrawingView : UserControl
{

    // status of draw operation
    private bool isActive;
    private Point currentPosition;
    private Point firstPosition;
    private Point lastPosition;
    

    public DrawingView()
    {
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

            handleDraw();
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


    private void handleDraw()
    {
        // just draw rectangle for now
       
        // canvas can be used as rendertarget
        Canvas canvas = new Canvas();
        Canvas.SetLeft(canvas, Math.Min(firstPosition.X, lastPosition.X));
        Canvas.SetTop(canvas, Math.Min(firstPosition.Y, lastPosition.Y));
        canvas.Width = Math.Abs(firstPosition.X - lastPosition.X);
        canvas.Height = Math.Abs(firstPosition.Y - lastPosition.Y);

        Rectangle canvasBackground = new Rectangle();
        Canvas.SetLeft(canvasBackground, 0);
        Canvas.SetTop(canvasBackground, 0);
        canvasBackground.Width = canvas.Width;
        canvasBackground.Height = canvas.Height;
        canvasBackground.Fill = Brushes.Transparent;
        canvasBackground.Stroke = Brushes.Black;
        canvasBackground.StrokeThickness = 1;

        canvas.Children.Add(canvasBackground);
        Display.Children.Add(canvas);

    }
}