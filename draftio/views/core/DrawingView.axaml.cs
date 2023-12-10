using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Threading;
using draftio.models.dtos;
using draftio.models.structs;
using draftio.services;
using draftio.viewmodels;
using System;

namespace draftio;

public partial class DrawingView : UserControl
{
    public DrawingViewModel ViewModel { get; private set; }
    private readonly RenderManager renderManager;
    private readonly ToolManager toolManager;


    // status of draw operation
    private bool isDraw;
    private bool isMove;
    private Point currentPosition;
    private Point firstPosition;
    private Point lastPosition;
    private Vector2 moveOffset = new Vector2(0, 0);
    private double scaleOverlay = 1.0;
    private double scaleDisplay = 1.0;
    private DispatcherTimer timer;
    private Canvas Overlay;

    private Point zoomOrigin;



    public DrawingView()
    {
        ViewModel = App.GetService<DrawingViewModel>();
        DataContext = ViewModel;

        renderManager = App.GetService<RenderManager>();
        toolManager = App.GetService<ToolManager>();

        InitializeComponent();
        Display.PointerPressed += OnPointerPressed;
        Display.PointerReleased += OnPointerReleased;
        Display.PointerMoved += OnPointerMoved;

        Overlay = new Canvas();
        Overlay.Width = 1920;
        Overlay.Height = 1080;
        Overlay.IsEnabled = false;
        Overlay.Background = Brushes.Transparent;
        //Overlay.Opacity = 0.3;
        Overlay.ZIndex = 2;


        Canvas.SetTop(Overlay, 0);
        Canvas.SetLeft(Overlay, 0);



        Display.Children.Add(Overlay);

        Display.PointerWheelChanged += DisplayZoom_OnPointerWheelChanged;


        //timer = new DispatcherTimer();
        //timer.Interval = TimeSpan.FromMilliseconds(60);
        //timer.Tick += Timer_Tick;
        //timer.Start();
    }

    

    private void OnPointerMoved(object? sender, PointerEventArgs e)
    {

        currentPosition = e.GetPosition(sender as Control);
        zoomOrigin = currentPosition;

        renderManager.RenderOverlay(Overlay, firstPosition, currentPosition, isDraw, isMove, ViewModel.SelectedObject, moveOffset);
    }

    

    private void OnPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        var point = e.GetCurrentPoint(sender as Control);


        if (isDraw)
        {
            isDraw = false;
            lastPosition = point.Position;

            handleDraw();
            draw();
        }
        if (isMove)
        {
            isMove = false;
            lastPosition = point.Position;

            handleTranslate();
            draw();
        }
    }

    private void OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        var point = e.GetCurrentPoint(sender as Control);
        var position = point.Position;

        if (point.Properties.IsLeftButtonPressed)
        {
            isDraw = true;
            firstPosition = position;
            ViewModel.CollisionDetectPoint(new Vector2(firstPosition.X, firstPosition.Y));
           
        }

        if (point.Properties.IsMiddleButtonPressed)
        {
            isMove = true;
            firstPosition = position;


            ViewModel.CollisionDetectPoint(new Vector2(firstPosition.X, firstPosition.Y));

            if(ViewModel.SelectedObject != null)
            {
                // first we need to find moveOffset so we can handle overlay movement draw properly 
                moveOffset = new Vector2(firstPosition.X - ViewModel.SelectedObject.X, firstPosition.Y - ViewModel.SelectedObject.Y);
            }
        }

        if (point.Properties.IsRightButtonPressed)
        {
            firstPosition = position;

            ViewModel.CollisionDetectPoint(new Vector2(firstPosition.X, firstPosition.Y));
        }
    }

    private void handleDraw()
    {
        // just draw rectangle for now

        // canvas can be used as rendertarget

        PassData passData = new PassData
        {
            X = Math.Min(firstPosition.X, lastPosition.X),
            Y = Math.Min(firstPosition.Y, lastPosition.Y),
            Width = Math.Abs(firstPosition.X - lastPosition.X),
            Height = Math.Abs(firstPosition.Y - lastPosition.Y),
            SelectedObjectType = toolManager.SelectedObjectType
        };

        ViewModel.AddObject(passData);
    }


    private void handleTranslate()
    {
        if (ViewModel.SelectedObject != null)
        {
            moveOffset = new Vector2(firstPosition.X - ViewModel.SelectedObject.X, firstPosition.Y - ViewModel.SelectedObject.Y);

            ViewModel.SelectedObject.X = lastPosition.X - moveOffset.X;
            ViewModel.SelectedObject.Y = lastPosition.Y - moveOffset.Y;
        }
    }


    private void draw()
    {
        Display.Children.Clear();
        Display.Children.Add(Overlay);


        renderManager.Render(Display, ViewModel.Objects);
    }


    private void DisplayZoom_OnPointerWheelChanged(object? sender, PointerWheelEventArgs e)
    {
        const double zoomSpeed = 0.1;
        double zoomFactor = e.Delta.Y > 0 ? (1 + zoomSpeed) : 1 / (1 + zoomSpeed);

        // Calculate the new scale
        double newScale = scaleDisplay * zoomFactor;

        // Limit the scale within certain bounds if necessary
        newScale = Math.Max(0.1, Math.Min(10, newScale));

        // Calculate the translation to keep the mouse position fixed
        Vector translation = new Vector(Display.Width / 2 - currentPosition.X, Display.Height / 2 - currentPosition.Y);
        translation *= 1 - 1 / zoomFactor;

        // Apply the scale and translation to the RenderTransform
        Display.RenderTransform = new TransformGroup
        {
            Children =
        {
            new TranslateTransform(-translation.X, -translation.Y),
            new ScaleTransform(newScale, newScale),
            new TranslateTransform(translation.X, translation.Y)
        }
        };

        // Update the scale for the next iteration
        scaleDisplay = newScale;
    }

   
}