using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
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

    // status of draw operation
    private bool isDraw;
    private bool isMove;
    private Point currentPosition;
    private Point firstPosition;
    private Point lastPosition;
    private Vector2 moveOffset = new Vector2(0, 0);

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
            Height = Math.Abs(firstPosition.Y - lastPosition.Y)
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

        renderManager.Render(Display, ViewModel.Objects);
    }
}