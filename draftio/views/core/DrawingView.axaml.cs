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
using System.Diagnostics;
using System.Drawing;
using System.Linq;

namespace draftio;

public partial class DrawingView : UserControl
{
    public DrawingViewModel ViewModel { get; private set; }
    private readonly RenderManager renderManager;
    private readonly ToolManager toolManager;


    // status of draw operation
    private bool isDraw;
    private bool isMove;
    private Avalonia.Point currentPosition;
    private Avalonia.Point firstPosition;
    private Avalonia.Point lastPosition;
    private Vector2 moveOffset = new Vector2(0, 0);
    private double scaleOverlay = 1.0;
    private double scaleDisplay = 1.0;
    private DispatcherTimer timer;
    private Canvas Overlay;


    private Avalonia.Point origin;
    private Avalonia.Point start;


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
        Overlay.Background = Avalonia.Media.Brushes.Transparent;
        //Overlay.Opacity = 0.3;
        Overlay.ZIndex = 2;


        Canvas.SetTop(Overlay, 0);
        Canvas.SetLeft(Overlay, 0);



        Display.Children.Add(Overlay);

        Display.PointerWheelChanged += DisplayZoom_OnPointerWheelChanged;



        // new ----------------------------------

        TransformGroup group = new TransformGroup();

        ScaleTransform xform = new ScaleTransform();
        group.Children.Add(xform);

        TranslateTransform tt = new TranslateTransform();
        group.Children.Add(tt);

        Display.RenderTransform = group;

        //timer = new DispatcherTimer();
        //timer.Interval = TimeSpan.FromMilliseconds(60);
        //timer.Tick += Timer_Tick;
        //timer.Start();
    }

    

    private void OnPointerMoved(object? sender, PointerEventArgs e)
    {
        var point = e.GetCurrentPoint(sender as Control);
        currentPosition = e.GetPosition(sender as Control);

        


        if (Display.IsPointerOver)
        {
            if (point.Properties.IsRightButtonPressed)
            {
                var tt = (TranslateTransform)((TransformGroup)Display.RenderTransform)
               .Children.First(tr => tr is TranslateTransform);
                Vector v = start - e.GetPosition(BorderMain);
                tt.X = origin.X - v.X;
                tt.Y = origin.Y - v.Y;
            }
        }
        




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

            var tt = (TranslateTransform)((TransformGroup)Display.RenderTransform)
                .Children.First(tr => tr is TranslateTransform);
            start = e.GetPosition(BorderMain);
            origin = new Avalonia.Point(tt.X, tt.Y);
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

    private Avalonia.Point lastMousePosition = new Avalonia.Point(0, 0);
    private void DisplayZoom_OnPointerWheelChanged(object? sender, PointerWheelEventArgs e)
    {
        //var position = e.GetPosition(sender as Control);




        //TransformGroup transformGroup = (TransformGroup)Display.RenderTransform;
        //ScaleTransform sTransform = (ScaleTransform)transformGroup.Children[0];
        //TranslateTransform tTransform = (TranslateTransform)((TransformGroup)Display.RenderTransform)
        //        .Children.First(tr => tr is TranslateTransform);


        //double zoom = e.Delta.Y > 0 ? .2 : -.2;

        //Avalonia.Point relative = e.GetPosition(sender as Control);
        //Display.RenderTransformOrigin = new RelativePoint(new Avalonia.Point((relative.X / Display.Bounds.Width), (relative.Y / Display.Bounds.Height)), RelativeUnit.Relative);




        //sTransform.ScaleX += zoom;
        //sTransform.ScaleY += zoom;







        TransformGroup transformGroup = (TransformGroup)Display.RenderTransform;
        ScaleTransform sTransform = (ScaleTransform)transformGroup.Children.First(tr => tr is ScaleTransform);
        TranslateTransform tTransform = (TranslateTransform)transformGroup.Children.First(tr => tr is TranslateTransform);

        double zoom = e.Delta.Y > 0 ? 0.2 : -0.2;

        Avalonia.Point currentMousePosition = e.GetPosition(Display);

        // Calculate the delta in mouse position since the last zoom
        double deltaX = currentMousePosition.X - lastMousePosition.X;
        double deltaY = currentMousePosition.Y - lastMousePosition.Y;

        // Update the last mouse position
        lastMousePosition = currentMousePosition;

        // Apply the scale to the content
        sTransform.ScaleX += zoom;
        sTransform.ScaleY += zoom;

        // Calculate the new translation
        double newTranslateX = tTransform.Value.M31 + deltaX * zoom;
        double newTranslateY = tTransform.Value.M32 + deltaY * zoom;

        // Apply the translation to the content
        tTransform = new TranslateTransform(newTranslateX, newTranslateY);

        // Update the RenderTransformOrigin to the new position
        Display.RenderTransformOrigin = new RelativePoint(new Avalonia.Point((currentMousePosition.X / Display.Bounds.Width), (currentMousePosition.Y / Display.Bounds.Height)), RelativeUnit.Relative);




        Trace.WriteLine(Display.RenderTransformOrigin);




        //const double zoomSpeed = 0.1;
        //double zoomFactor = e.Delta.Y > 0 ? (1 + zoomSpeed) : 1 / (1 + zoomSpeed);

        //// Calculate the new scale
        //double newScale = scaleDisplay * zoomFactor;

        //// Limit the scale within certain bounds if necessary
        //newScale = Math.Max(0.1, Math.Min(10, newScale));

        //// Calculate the translation to keep the mouse position fixed
        //Vector translation = new Vector(Display.Width / 2 - currentPosition.X, Display.Height / 2 - currentPosition.Y);
        //translation *= 1 - 1 / zoomFactor;

        //// Apply the scale and translation to the RenderTransform
        //Display.RenderTransform = new TransformGroup
        //{
        //    Children =
        //{
        //    new TranslateTransform(-translation.X, -translation.Y),
        //    new ScaleTransform(newScale, newScale),
        //    new TranslateTransform(translation.X, translation.Y)
        //}
        //};

        //// Update the scale for the next iteration
        //scaleDisplay = newScale;
    }





}