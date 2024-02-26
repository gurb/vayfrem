using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Threading;
using vayfrem.models;
using vayfrem.models.dtos;
using vayfrem.models.enums;
using vayfrem.models.objects.@base;
using vayfrem.models.structs;
using vayfrem.services;
using vayfrem.viewmodels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using Avalonia.Animation;
using vayfrem.models.objects;
using Avalonia.Controls.PanAndZoom;

namespace vayfrem;

public partial class DrawingView : UserControl
{
    public DrawingViewModel ViewModel { get; private set; }
    public LayoutViewModel layoutViewModel { get; private set; }
    private readonly ProjectManager projectManager;
    private readonly RenderManager renderManager;
    private readonly ToolManager toolManager;
    private readonly ObjectMenuManager objectMenuManager;

    // status of draw operation
    private bool isDraw;
    private bool isMove;

    private bool isRightClick;
    
    private Avalonia.Point oldCurrentPosition;
    private Avalonia.Point currentPosition;
    private Avalonia.Point firstPosition;
    private Avalonia.Point lastPosition;
    private Vector2 moveOffset = new Vector2(0, 0);
    private double scaleOverlay = 1.0;
    private double scaleDisplay = 1.0;
    private DispatcherTimer timer;
    //private Canvas Overlay;


    private Avalonia.Point origin;
    private Avalonia.Point start;
    ScaleTransform xform;

    TimeSpan lastClickTime = new TimeSpan();
    Node? lastClickNode;



    // Polygon selection
    private Polygon triangle = new Polygon();
    Avalonia.Controls.Shapes.Rectangle startPointRect = new Avalonia.Controls.Shapes.Rectangle();
    Avalonia.Controls.Shapes.Rectangle point1Rect = new Avalonia.Controls.Shapes.Rectangle();
    Avalonia.Controls.Shapes.Rectangle point2Rect = new Avalonia.Controls.Shapes.Rectangle();
    
    Avalonia.Controls.Shapes.Rectangle? selectedPoint;
    List<Avalonia.Controls.Shapes.Rectangle> triangleRects = new List<Avalonia.Controls.Shapes.Rectangle>();


    public DrawingView()
    {
        ViewModel = App.GetService<DrawingViewModel>();
        ViewModel.drawDelegate += draw;
        ViewModel.setDimensionDelegate += SetDimension;
        DataContext = ViewModel;

        layoutViewModel = App.GetService<LayoutViewModel>();
        
        projectManager = App.GetService<ProjectManager>();
        //projectManager.setDimensionDelegate += SetDimension;
        renderManager = App.GetService<RenderManager>();
        toolManager = App.GetService<ToolManager>();
        objectMenuManager = App.GetService<ObjectMenuManager>();

        InitializeComponent();

        this.SizeChanged += DrawingView_SizeChanged;

        Display.PointerPressed += OnPointerPressed;
        Display.PointerReleased += OnPointerReleased;
        Display.PointerMoved += OnPointerMoved;
        Display.PointerWheelChanged += Display_PointerWheelChanged;

        this.PointerReleased += DrawingView_PointerReleased;

        //Overlay = new Canvas();
        Overlay.Width = 1920;
        Overlay.Height = 1080;
        Overlay.IsEnabled = false;
        Overlay.Background = Avalonia.Media.Brushes.Transparent;
        Overlay.ClipToBounds = false;
        ////Overlay.Opacity = 0.3;
        //Overlay.ZIndex = 2;

        Canvas.SetTop(Overlay, 0);
        Canvas.SetLeft(Overlay, 0);

        //Display.Children.Add(Overlay);

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

        SetPolygonTriangle();

        AfterInit();
        renderManager.SetMainDisplay(Display);
        //SetObjectMenu();
    }
    private void DrawingView_SizeChanged(object? sender, SizeChangedEventArgs e)
    {
        if (ViewModel.setDimensionDelegate != null)
        {
            ViewModel.setDimensionDelegate.Invoke();
        }
    }

    private void DrawingView_PointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        var test = 0;
    }

    private void SetDimension()
    {
        if(ViewModel.CurrentFile != null)
        {
            Display.Width = ViewModel.CurrentFile.PageWidth;
            Display.Height = ViewModel.CurrentFile.PageHeight;
        }
    }
    private void Display_PointerWheelChanged(object? sender, PointerWheelEventArgs e)
    {
        var point = e.GetCurrentPoint(sender as Control);
        currentPosition = e.GetPosition(sender as Control);
        currentPosition = new Avalonia.Point((int)currentPosition.X, (int)currentPosition.Y);

        renderManager.Zoom = ZoomBorder1.ZoomX;

        UpdateRects(ZoomBorder1.ZoomX);
        renderManager.RenderOverlay(Overlay, firstPosition, currentPosition, isDraw, isMove, ViewModel.SelectedObject, moveOffset);
        DrawTriangleSelection();
    }

    private void AfterInit()
    {
        draw();
    }
    
    private void OnPointerMoved(object? sender, PointerEventArgs e)
    {
        var point = e.GetCurrentPoint(sender as Control);
        currentPosition = e.GetPosition(sender as Control);
        currentPosition = new Avalonia.Point((int)currentPosition.X, (int)currentPosition.Y);

        if (!layoutViewModel.IsDragCompleted)
        {
            if (layoutViewModel.DragObject != null)
            {
                GObject? parentObject = ViewModel.CollisionPointWithObject(new Vector2(point.Position.X, point.Position.Y), null);

                layoutViewModel.DragObject.X = (int)point.Position.X - layoutViewModel.DragObject.Width / 2;
                layoutViewModel.DragObject.Y = (int)point.Position.Y - layoutViewModel.DragObject.Height / 2;
                ViewModel.AddDirectObject(layoutViewModel.DragObject, parentObject);
                layoutViewModel.DragObject = null;
            }
            layoutViewModel.IsDragCompleted = true;
        }

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
        DrawTriangleSelection();
    }
    
    private void OnPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        var point = e.GetCurrentPoint(sender as Control);

        if (isDraw)
        {
            isDraw = false;
            lastPosition = new Avalonia.Point((int)point.Position.X, (int)point.Position.Y);

            handleDraw();
            draw();
        }

        if(isMove && Math.Abs(firstPosition.X - point.Position.X) < 1 && Math.Abs(firstPosition.Y - point.Position.Y) < 1)
        {
            isMove = false;
            lastPosition = new Avalonia.Point((int)point.Position.X, (int)point.Position.Y); ;
            if (!isMove && isRightClick)
            {
                ViewModel.CollisionDetectPoint(new Vector2(point.Position.X, point.Position.Y));

                if (ViewModel.SelectedObject != null)
                {
                    objectMenuManager.SetObject(ViewModel.SelectedObject);
                }
                layoutViewModel.IsOpenMenu = true;
                isRightClick = false;
                return;
            }
        }

        if (isMove)
        {
            isMove = false;
            lastPosition = new Avalonia.Point((int)point.Position.X, (int)point.Position.Y); ;

            handleTranslate();
            draw();
        }

        if(ViewModel.IsScale)
        {
            ViewModel.IsScale = false;
            ViewModel.RefreshState();
            ViewModel.Reconf();
            draw();
        }
    }

    private void OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        var point = e.GetCurrentPoint(sender as Control);
        var position = new Avalonia.Point((int)point.Position.X, (int)point.Position.Y);
        oldCurrentPosition = currentPosition;
        currentPosition = position;

        
        CollisionDetectPoint(position);

        if (point.Properties.IsRightButtonPressed)
        {
            isRightClick = true;
        }

        if (point.Properties.IsLeftButtonPressed && toolManager.SelectedToolOption == ToolOption.Select && !ViewModel.IsOverScalePoint  && !ViewModel.IsScale && !ViewModel.IsOverQBCScalePoint)
        {
            ViewModel.IsSelect = true;
            
            firstPosition = position;

            ViewModel.CollisionDetectPoint(new Vector2(firstPosition.X, firstPosition.Y));

            handleSelection();
        }


        if (point.Properties.IsLeftButtonPressed && toolManager.SelectedToolOption == ToolOption.Select && ViewModel.IsSelect && (ViewModel.IsOverScalePoint || ViewModel.IsOverQBCScalePoint) && !ViewModel.IsScale)
        {
            ViewModel.IsScale = true;

        }

        if (point.Properties.IsLeftButtonPressed 
        && 
        (
            toolManager.SelectedToolOption == ToolOption.Rect ||
            toolManager.SelectedToolOption == ToolOption.Text ||
            toolManager.SelectedToolOption == ToolOption.QBC
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
            SelectedObjectType = toolManager.SelectedObjectType,
        };

        if (toolManager.SelectedObjectType == ObjectType.QuadraticBC)
        {
            passData.StartPoint = new Vector2((int)firstPosition.X, (int)firstPosition.Y);
            passData.Point2 = new Vector2((int)lastPosition.X, (int)lastPosition.Y);

            int center_x = ((int)passData.StartPoint.X + (int)passData.Point2.X) / 2;
            int center_y = ((int)passData.StartPoint.Y + (int)passData.Point2.Y) / 2;

            passData.Point1 = new Vector2(center_x, center_y);
        }

        ViewModel.AddObject(passData);
    }


    private void handleTranslate()
    {
        if (ViewModel.SelectedObject != null)
        {
            moveOffset = new Vector2(firstPosition.X - ViewModel.SelectedObject.X, firstPosition.Y - ViewModel.SelectedObject.Y);

            ViewModel.SelectedObject.X = (int)(lastPosition.X - moveOffset.X);
            ViewModel.SelectedObject.Y = (int)(lastPosition.Y - moveOffset.Y);

            if(ViewModel.SelectedObject.ObjectType == ObjectType.QuadraticBC)
            {
                QuadraticBCObj qbc = (QuadraticBCObj)ViewModel.SelectedObject;
                qbc.X = 0;
                qbc.Y = 0;

                moveOffset = new Vector2(firstPosition.X - qbc.StartPoint.X, firstPosition.Y - qbc.StartPoint.Y);

                Vector2 old = new Vector2(qbc.StartPoint);

                qbc.StartPoint.X = (int)(lastPosition.X - moveOffset.X);
                qbc.StartPoint.Y = (int)(lastPosition.Y - moveOffset.Y);
                Vector2 delta = old - qbc.StartPoint;

                qbc.Point1 = qbc.Point1 - delta;
                qbc.Point2 = qbc.Point2 - delta;
            }

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
        if(obj != null && obj.ObjectType == ObjectType.QuadraticBC && selectedPoint != null)
        {
            Avalonia.Point offsetPosition = currentPosition - new Avalonia.Point(obj.WorldX, obj.WorldY);

            Avalonia.Point currentPos = offsetPosition;

            QuadraticBCObj qbcObj = (QuadraticBCObj)obj;

            Canvas.SetLeft(selectedPoint, (int)currentPos.X);
            Canvas.SetTop(selectedPoint, (int)currentPos.Y);

            if (selectedPoint.Name == "StartPoint")
            {
                qbcObj.StartPoint = new Vector2(currentPos.X, currentPos.Y);
            }
            if (selectedPoint.Name == "Point1")
            {
                qbcObj.Point1 = new Vector2(currentPos.X, currentPos.Y);
            }
            if (selectedPoint.Name == "Point2")
            {
                qbcObj.Point2 = new Vector2(currentPos.X, currentPos.Y);
            }

        }
        else if (obj != null)
        {
            
            string? scalePointType = ViewModel.GetOverScalePoint;

            Avalonia.Point currentPos = currentPosition;

            double old_X = (int)obj.X;
            double old_Y = (int)obj.Y;

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
                        obj.Width = (int)(obj.Width - (currentPos.X - obj.X));
                    }
                    else
                    {
                        obj.Width = (int)(obj.Width + (obj.X - currentPos.X));
                    }
                    if (currentPos.Y > obj.Y)
                    {
                        obj.Height = (int)(obj.Height - (currentPos.Y - obj.Y));
                    }
                    else
                    {
                        obj.Height = (int)(obj.Height + (obj.Y - currentPos.Y));
                    }
                    
                    obj.X = (int)currentPos.X;
                    obj.Y = (int)currentPos.Y;
                }
                
                if(scalePointType == "TopCenter")
                {
                    if (currentPos.Y > obj.Y)
                    {
                        obj.Height = (int)(obj.Height - (currentPos.Y - obj.Y));
                    }
                    else
                    {
                        obj.Height = (int)(obj.Height + (obj.Y - currentPos.Y));
                    }
                    
                    obj.Y = (int)currentPos.Y;
                }
                if(scalePointType == "TopRight")
                {
                    if (currentPos.X > obj.X + obj.Width)
                    {
                        obj.Width = (int)Math.Abs(currentPos.X - obj.X);
                    }
                    else
                    {
                        obj.Width = (int)Math.Abs(currentPos.X - obj.X);
                    }
                    if (currentPos.Y > obj.Y)
                    {
                        obj.Height = (int)(obj.Height + (obj.Y - currentPos.Y));
                    }
                    else
                    {
                        obj.Height = (int)(obj.Height + (obj.Y - currentPos.Y));
                    }
                    
                    obj.Y = (int)currentPos.Y;
                }

                if(scalePointType == "MiddleLeft")
                {
                    if (currentPos.X > obj.X)
                    {
                        obj.Width = (int)(obj.Width - (currentPos.X - obj.X));
                    }
                    else
                    {
                        obj.Width = (int)(obj.Width + (obj.X - currentPos.X));
                    }

                    obj.X = (int)currentPos.X;
                }

                if (scalePointType == "MiddleRight")
                {
                    if (currentPos.X > obj.X + Width)
                    {
                        obj.Width = (int)Math.Abs(currentPos.X - obj.X);
                    }
                    else
                    {
                        obj.Width = (int)Math.Abs(currentPos.X - obj.X);
                    }
                }

                if(scalePointType == "BottomLeft")
                {
                    if (currentPos.X > obj.X)
                    {
                        obj.Width = (int)(obj.Width - (currentPos.X - obj.X));
                    }
                    else
                    {
                        obj.Width = (int)(obj.Width + (obj.X - currentPos.X));
                    }
                    if (currentPos.Y > obj.Y)
                    {
                        obj.Height = (int)Math.Abs(currentPos.Y - obj.Y);
                    }
                    else
                    {
                        obj.Height = (int)Math.Abs(obj.Y - currentPos.Y);
                    }
                    obj.X = (int)currentPos.X;
                }

                if(scalePointType == "BottomCenter")
                {
                    obj.Height = (int)Math.Abs(currentPos.Y - obj.Y);
                }

                if(scalePointType == "BottomRight")
                {
                    obj.Width = (int)Math.Abs(currentPos.X - obj.X);
                    obj.Height = (int)Math.Abs(currentPos.Y - obj.Y);
                }



                // note: we comment this because of performance issue
                //ViewModel.RefreshState();

                if (obj.Width <= 50)
                {
                    obj.X = (int)old_X;
                    obj.Width = 51;
                    return;
                }
                if (obj.Height <= 50)
                {
                    obj.Y = (int)old_Y;
                    obj.Height = 51;
                    return;
                }


            }
        }
    }

    private void draw()
    {
        ZoomBorder1.Child = null;
        Display.Children.Clear();

        if(!ViewModel.IsEmpty)
        {
            Display.IsVisible = true;
            //Display.Children.Add(Overlay);

            renderManager.Render(Display, ViewModel.Objects);
            renderManager.RenderOverlay(Overlay, firstPosition, currentPosition, isDraw, isMove, ViewModel.SelectedObject, moveOffset);
            DrawTriangleSelection();
        } else
        {
            Display.IsVisible = false;
        }

        ZoomBorder1.Child = Main;
    }

    public static Avalonia.Matrix ScaleAt(double scaleX, double scaleY, double centerX, double centerY)
    {
        return new Avalonia.Matrix(scaleX, 0, 0, scaleY, centerX - (scaleX * centerX), centerY - (scaleY * centerY));
    }

    //
    private void SetPolygonTriangle()
    {
        triangle.StrokeDashOffset = 1;

        triangle.StrokeThickness = 1;
        triangle.Fill = Avalonia.Media.Brushes.Transparent;
        triangle.Stroke = Avalonia.Media.Brushes.Black;

        triangle.Stroke = Avalonia.Media.Brushes.Black;
        triangle.StrokeThickness = 1;
        var dashStyle = new Avalonia.Media.DashStyle(new double[] { 2, 2 }, 0);
        triangle.StrokeDashArray = dashStyle.Dashes;

        startPointRect.Name = "StartPoint";
        point1Rect.Name = "Point1";
        point2Rect.Name = "Point2";

        startPointRect.PointerPressed += TrianglePointRect_PointerPressed;
        point1Rect.PointerPressed += TrianglePointRect_PointerPressed;
        point2Rect.PointerPressed += TrianglePointRect_PointerPressed;

        triangleRects.Add(startPointRect);
        triangleRects.Add(point1Rect);
        triangleRects.Add(point2Rect);

        Avalonia.Controls.Shapes.Rectangle Sample = new Avalonia.Controls.Shapes.Rectangle();

        Sample.Width = 10;
        Sample.Height = 10;
        Sample.StrokeThickness = 1;
        Sample.Stroke = Avalonia.Media.Brushes.Black;

        foreach (var rect in triangleRects)
        {
            rect.Width = Sample.Width;
            rect.Height = Sample.Height;
            rect.Fill = Avalonia.Media.Brushes.White;
            rect.StrokeThickness = Sample.StrokeThickness;
            rect.Stroke = Sample.Stroke;
        }
    }

    private void TrianglePointRect_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        Avalonia.Controls.Shapes.Rectangle rect = sender as Avalonia.Controls.Shapes.Rectangle;

        ViewModel.IsScale = true;

        if (rect.Name == "StartPoint")
        {
            selectedPoint = rect;
        }
        if (rect.Name == "Point1")
        {
            selectedPoint = rect;
        }
        if (rect.Name == "Point2")
        {
            selectedPoint = rect;
        }
    }

    public void CollisionDetectPoint(Avalonia.Point mousePosition)
    {
        if (ViewModel.IsScale)
        {
            return;
        }

        foreach (var obj in triangleRects)
        {
            if (mousePosition.X >= obj.Bounds.TopLeft.X &&
                mousePosition.X <= obj.Bounds.TopLeft.X + obj.Bounds.Width &&
                mousePosition.Y >= obj.Bounds.TopLeft.Y &&
                mousePosition.Y <= obj.Bounds.TopLeft.Y + obj.Bounds.Height)
            {
                //SetCursorAccordingToRectangle(obj);
                selectedPoint = obj;
                ViewModel.IsOverQBCScalePoint = true;
                return;
            }
        }
        ViewModel.IsOverQBCScalePoint = false;
    }

    private void UpdateRects(double zoom)
    {
        double width = 10;
        double height = 10;
        double thickness = 1;
        double borderThickness = 2;

        if (zoom != 1)
        {

            double size = (10 / (double)zoom);
            width = size;
            height = size;
            thickness = 1 / (double)zoom;
            borderThickness = 2 / (double)zoom;
        }

        foreach (var rect in triangleRects)
        {
            rect.Width = width;
            rect.Height = height;
            rect.StrokeThickness = thickness;
        }

    }

    private void DrawTriangleSelection()
    {
        if(ViewModel.CurrentFile != null && ViewModel.CurrentFile.Selection!.SelectedObject != null && ViewModel.CurrentFile.Selection!.SelectedObject.ObjectType == ObjectType.QuadraticBC)
        {
            QuadraticBCObj quadraticBCObj = (QuadraticBCObj)ViewModel.CurrentFile.Selection.SelectedObject;

            Canvas.SetLeft(startPointRect, quadraticBCObj.StartPoint.X + quadraticBCObj.WorldX);
            Canvas.SetTop(startPointRect, quadraticBCObj.StartPoint.Y + quadraticBCObj.WorldY);

            Canvas.SetLeft(point1Rect, quadraticBCObj.Point1.X + quadraticBCObj.WorldX);
            Canvas.SetTop(point1Rect, quadraticBCObj.Point1.Y + quadraticBCObj.WorldY);

            Canvas.SetLeft(point2Rect, quadraticBCObj.Point2.X + quadraticBCObj.WorldX);
            Canvas.SetTop(point2Rect, quadraticBCObj.Point2.Y + quadraticBCObj.WorldY);

            triangle.Points = new Points()
            {
                new Avalonia.Point(quadraticBCObj.StartPoint.X + quadraticBCObj.WorldX, quadraticBCObj.StartPoint.Y + quadraticBCObj.WorldY),
                new Avalonia.Point(quadraticBCObj.Point1.X + quadraticBCObj.WorldX, quadraticBCObj.Point1.Y + quadraticBCObj.WorldY),
                new Avalonia.Point(quadraticBCObj.Point2.X + quadraticBCObj.WorldX, quadraticBCObj.Point2.Y + quadraticBCObj.WorldY),
            };

            Overlay.Children.Add(triangle);
            Overlay.Children.Add(startPointRect);
            Overlay.Children.Add(point1Rect);
            Overlay.Children.Add(point2Rect);
        }
    }
}