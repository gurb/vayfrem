using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Threading;
using draftio.models;
using draftio.models.dtos;
using draftio.models.enums;
using draftio.models.objects.@base;
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
    
    private Avalonia.Point oldCurrentPosition;
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

    TimeSpan lastClickTime = new TimeSpan();
    Node? lastClickNode;



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
        Display.PointerWheelChanged += Display_PointerWheelChanged;

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
        AfterInit();
        renderManager.SetMainDisplay(Display);
    }

    private void Display_PointerWheelChanged(object? sender, PointerWheelEventArgs e)
    {
        var point = e.GetCurrentPoint(sender as Control);
        currentPosition = e.GetPosition(sender as Control);

        renderManager.Zoom = ZoomBorder.ZoomX;

        renderManager.RenderOverlay(Overlay, firstPosition, currentPosition, isDraw, isMove, ViewModel.SelectedObject, moveOffset);
    }

    private void AfterInit()
    {
        draw();
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

        if(ViewModel.IsScale)
        {
            handleScale();
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

        if(ViewModel.IsScale)
        {
            ViewModel.IsScale = false;
            draw();
        }
    }

    private void OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        var point = e.GetCurrentPoint(sender as Control);
        var position = point.Position;
        oldCurrentPosition = currentPosition;
        currentPosition = position;



        if (point.Properties.IsLeftButtonPressed && toolManager.SelectedToolOption == ToolOption.Select && !ViewModel.IsOverScalePoint && !ViewModel.IsScale)
        {
            ViewModel.IsSelect = true;
            
            firstPosition = position;
            ViewModel.CollisionDetectPoint(new Vector2(firstPosition.X, firstPosition.Y));
            handleSelection();
        }

        if(point.Properties.IsLeftButtonPressed && toolManager.SelectedToolOption == ToolOption.Select && ViewModel.IsSelect && ViewModel.IsOverScalePoint && !ViewModel.IsScale)
        {
            ViewModel.IsScale = true;
        }

        


        if (point.Properties.IsLeftButtonPressed 
        && 
        (
            toolManager.SelectedToolOption == ToolOption.Rect ||
            toolManager.SelectedToolOption == ToolOption.Text
        ))
        {
            isDraw = true;
            firstPosition = position;
            ViewModel.CollisionDetectPoint(new Vector2(firstPosition.X, firstPosition.Y));
        }

        TimeSpan timeSinceLastClick = DateTime.Now.TimeOfDay - lastClickTime;
        if (point.Properties.IsLeftButtonPressed && toolManager.SelectedToolOption == ToolOption.Text && oldCurrentPosition == currentPosition && timeSinceLastClick.TotalMilliseconds < 300)
        {
            if (ViewModel.SelectedObject != null && ViewModel.SelectedObject.ObjectType == ObjectType.Text)
            {
                ViewModel.ActiveEditText(ViewModel.SelectedObject);
            }
        }

        if(point.Properties.IsLeftButtonPressed && toolManager.SelectedToolOption == ToolOption.Text)
        {
            lastClickTime = DateTime.Now.TimeOfDay;
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
            ViewModel.RefreshState();
        }
    }

    private void handleSelection()
    {
        if (ViewModel.SelectedObject != null)
        {
            ViewModel.SetSelectedObject(ViewModel.SelectedObject);
        }else
        {
            ViewModel.SetSelectedObject(null);
        }
    }

    private void handleScale()
    {
        GObject? obj = ViewModel.GetSelectionObject();
        if (obj != null)
        {
            
            string? scalePointType = ViewModel.GetOverScalePoint;

            Avalonia.Point currentPos = currentPosition;

            double old_X = obj.X;
            double old_Y = obj.Y;

            if(obj.Parent != null)
            {
                currentPos = new Avalonia.Point(currentPos.X - obj.Parent.WorldX, currentPos.Y - obj.Parent.WorldY);
            }

            if(currentPos.X < obj.X && (scalePointType ==  "TopRight" || scalePointType == "MiddleRight" || scalePointType == "BottomRight"))
            {
                return;
            }
            if(currentPos.Y < obj.Y && (scalePointType == "BottomLeft" || scalePointType == "BottomCenter" || scalePointType == "BottomRight")) 
            { 
                return; 
            }

            if(obj != null && scalePointType != null)
            {

                if(scalePointType == "TopLeft")
                {
                    if (currentPos.X > obj.X)
                    {
                        obj.Width = obj.Width - (currentPos.X - obj.X);
                    }
                    else
                    {
                        obj.Width = obj.Width + (obj.X - currentPos.X);
                    }
                    if (currentPos.Y > obj.Y)
                    {
                        obj.Height = obj.Height - (currentPos.Y - obj.Y);
                    }
                    else
                    {
                        obj.Height = obj.Height + (obj.Y - currentPos.Y);
                    }
                    
                    obj.X = currentPos.X;
                    obj.Y = currentPos.Y;
                }
                
                if(scalePointType == "TopCenter")
                {
                    if (currentPos.Y > obj.Y)
                    {
                        obj.Height = obj.Height - (currentPos.Y - obj.Y);
                    }
                    else
                    {
                        obj.Height = obj.Height + (obj.Y - currentPos.Y);
                    }
                    
                    obj.Y = currentPos.Y;
                }
                if(scalePointType == "TopRight")
                {
                    if (currentPos.X > obj.X + obj.Width)
                    {
                        obj.Width = Math.Abs(currentPos.X - obj.X);
                    }
                    else
                    {
                        obj.Width = Math.Abs(currentPos.X - obj.X);
                    }
                    if (currentPos.Y > obj.Y)
                    {
                        obj.Height = obj.Height + (obj.Y - currentPos.Y);
                    }
                    else
                    {
                        obj.Height = obj.Height + (obj.Y - currentPos.Y);
                    }
                    
                    obj.Y = currentPos.Y;
                }

                if(scalePointType == "MiddleLeft")
                {
                    if (currentPos.X > obj.X)
                    {
                        obj.Width = obj.Width - (currentPos.X - obj.X);
                    }
                    else
                    {
                        obj.Width = obj.Width + (obj.X - currentPos.X);
                    }

                    obj.X = currentPos.X;
                }

                if (scalePointType == "MiddleRight")
                {
                    if (currentPos.X > obj.X + Width)
                    {
                        obj.Width = Math.Abs(currentPos.X - obj.X);
                    }
                    else
                    {
                        obj.Width = Math.Abs(currentPos.X - obj.X);
                    }
                }

                if(scalePointType == "BottomLeft")
                {
                    if (currentPos.X > obj.X)
                    {
                        obj.Width = obj.Width - (currentPos.X - obj.X);
                    }
                    else
                    {
                        obj.Width = obj.Width + (obj.X - currentPos.X);
                    }
                    if (currentPos.Y > obj.Y)
                    {
                        obj.Height = Math.Abs(currentPos.Y - obj.Y);
                    }
                    else
                    {
                        obj.Height = Math.Abs(obj.Y - currentPos.Y);
                    }
                    obj.X = currentPos.X;
                }

                if(scalePointType == "BottomCenter")
                {
                    obj.Height = Math.Abs(currentPos.Y - obj.Y);
                }

                if(scalePointType == "BottomRight")
                {
                    obj.Width = Math.Abs(currentPos.X - obj.X);
                    obj.Height = Math.Abs(currentPos.Y - obj.Y);
                }

                ViewModel.RefreshState();

                if (obj.Width <= 50)
                {
                    obj.X = old_X;
                    obj.Width = 51;
                    return;
                }
                if (obj.Height <= 50)
                {
                    obj.Y = old_Y;
                    obj.Height = 51;
                    return;
                }
            }
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
            renderManager.RenderOverlay(Overlay, firstPosition, currentPosition, isDraw, isMove, ViewModel.SelectedObject, moveOffset);
        } else
        {
            Display.IsVisible = false;
        }
    }

    public static Avalonia.Matrix ScaleAt(double scaleX, double scaleY, double centerX, double centerY)
    {
        return new Avalonia.Matrix(scaleX, 0, 0, scaleY, centerX - (scaleX * centerX), centerY - (scaleY * centerY));
    }
}