using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using vayfrem.services;
using vayfrem.models.enums;
using System.Drawing;
using vayfrem.viewmodels;

namespace vayfrem;

public partial class ToolbarView : UserControl
{
    private readonly ToolManager toolManager;
    private readonly PageTreeViewModel pageTreeViewModel;

    Button selectedButton;

    public ToolbarView()
    {
        toolManager = App.GetService<ToolManager>();
        pageTreeViewModel = App.GetService<PageTreeViewModel>();
        pageTreeViewModel.setSelect += SetSelectTool;
        

        InitializeComponent();
        selectedButton = RectTool;

        SelectTool.Click += SelectTool_Click;
        RectTool.Click += RectTool_Click;
        TextTool.Click += TextTool_Click;
        ZoomInTool.Click += ZoomInTool_Click;
        ZoomOutTool.Click += ZoomOutTool_Click;
    }

    private void SelectTool_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        setButton(SelectTool);
        toolManager.SetToolOption(ToolOption.Select);
    }

    public void SetSelectTool()
    {
        setButton(SelectTool);
        toolManager.SetToolOption(ToolOption.Select);
    }

    private void TextTool_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        setButton(TextTool);
        toolManager.SetToolOption(ToolOption.Text);
    }

    private void ZoomOutTool_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        toolManager.SetToolOption(ToolOption.ZoomOut);
    }

    private void ZoomInTool_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        toolManager.SetToolOption(ToolOption.ZoomIn);
    }

    private void RectTool_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        setButton(RectTool);
        toolManager.SetToolOption(ToolOption.Rect);
    }

    private void setButton(Button button)
    {
        selectedButton.Classes.Clear();
        selectedButton = button;
        selectedButton.Classes.Add("Selected");
    }
}