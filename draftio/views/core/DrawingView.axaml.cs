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
using System.Drawing.Drawing2D;
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
    ScaleTransform xform;

    public DrawingView()
    {
        ViewModel = App.GetService<DrawingViewModel>();
        ViewModel.drawDelegate += draw;
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

        Display.IsVisible = false;



        // new ----------------------------------

        TransformGroup group = new TransformGroup();

        xform = new ScaleTransform();
        group.Children.Add(xform);

        TranslateTransform tt = new TranslateTransform();
        group.Children.Add(tt);

        Display.RenderTransform = group;

        //timer = new DispatcherTimer();
        //timer.Interval = TimeSpan.FromMilliseconds(60);
        //timer.Tick += Timer_Tick;
        //timer.Start();
        InitZoom();
    }


    private void InitZoom()
    {
        
    }

    

    private void OnPointerMoved(object? sender, PointerEventArgs e)
    {
        var point = e.GetCurrentPoint(sender as Control);
        currentPosition = e.GetPosition(sender as Control);

        


        if (Display.IsPointerOver)
        {
            if (point.Properties.IsRightButtonPressed)
            {
               // var tt = (TranslateTransform)((TransformGroup)Display.RenderTransform)
               //.Children.First(tr => tr is TranslateTransform);
               // Vector v = start - e.GetPosition(BorderMain);
               // tt.X = origin.X - v.X;
               // tt.Y = origin.Y - v.Y;
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

        if (point.Properties.IsRightButtonPressed)
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
        
        if (point.Properties.IsMiddleButtonPressed)
        {
            firstPosition = position;

            ViewModel.CollisionDetectPoint(new Vector2(firstPosition.X, firstPosition.Y));

            //var tt = (TranslateTransform)((TransformGroup)Display.RenderTransform)
            //    .Children.First(tr => tr is TranslateTransform);
            //start = e.GetPosition(BorderMain);
            //origin = new Avalonia.Point(tt.X, tt.Y);
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

        if(!ViewModel.IsEmpty)
        {
            Display.IsVisible = true;
            Display.Children.Add(Overlay);

            renderManager.Render(Display, ViewModel.Objects);
        } else
        {
            Display.IsVisible = false;
        }
    }

    private Avalonia.Point lastMousePosition = new Avalonia.Point(0, 0);
    //private System.Drawing.Drawing2D.Matrix matrix = Matrix.Identity;
    private double totalScale = 1.0;
    private double scaleDelta = 0.1;

    private double zoomFactor = 1.0;

    private float scale = 1.0f;

    private void DisplayZoom_OnPointerWheelChanged(object? sender, PointerWheelEventArgs e)
    {



        //BorderMain.RenderTransformOrigin = new RelativePoint(position.X / 1920, position.Y / 1080, RelativeUnit.Relative);




        //xform.ScaleX = 1;
        //xform.ScaleY = 1;
        //Display.RenderTransformOrigin = new RelativePoint(0.5, 0.5, RelativeUnit.Relative);


        //var position = e.GetPosition(sender as Control);

        //Display.RenderTransformOrigin = new RelativePoint(position.X / 1920, position.Y / 1080, RelativeUnit.Relative);

        //xform.ScaleX = 1;
        //xform.ScaleY = 1;


        //st = ScaleAt(scale, scale, pos1.X, pos1.Y) * st.Value;





        //// Ölçekleme faktörü



        //if (e.Delta.Y > 0)
        //{
        //    scale = scale + 0.1f;
        //} 
        //else
        //{
        //    scale = scale - 0.1f;
        //}

        //if (scale < 0.2f)
        //{
        //    scale = 0.2f;
        //} 
        //else if (scale > 1.9f)
        //{
        //    scale = 1.9f;
        //}
        //else
        //{
        //    xform.ScaleX = scale;
        //    xform.ScaleY = scale;
        //    Display.RenderTransformOrigin = new RelativePoint(position.X / 1920, position.Y / 1080, RelativeUnit.Relative);
        //}








        //var tt = (TranslateTransform)((TransformGroup)Display.RenderTransform)
        //      .Children.First(tr => tr is TranslateTransform);
        //tt.X = position.X / 1920;
        //tt.Y = position.Y / 1080;

        //Display.RenderTransformOrigin = new RelativePoint(position.X / 1920, position.Y / 1080, RelativeUnit.Relative);


    }

    public static Avalonia.Matrix ScaleAt(double scaleX, double scaleY, double centerX, double centerY)
    {
        return new Avalonia.Matrix(scaleX, 0, 0, scaleY, centerX - (scaleX * centerX), centerY - (scaleY * centerY));
    }





}