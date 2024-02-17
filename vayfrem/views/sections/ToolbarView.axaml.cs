using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using vayfrem.services;
using vayfrem.models.enums;
using System.Drawing;
using vayfrem.viewmodels;
using SFML.Graphics;

namespace vayfrem;

public partial class ToolbarView : UserControl
{
    private readonly ToolManager toolManager;
    private readonly PageTreeViewModel pageTreeViewModel;

    Button selectedButton;

    public ToolbarView()
    {
        toolManager = App.GetService<ToolManager>();
        toolManager.drawToolBarDelegate += SetButton;
        pageTreeViewModel = App.GetService<PageTreeViewModel>();
        pageTreeViewModel.setSelect += SetSelectTool;


        InitializeComponent();
        selectedButton = RectTool;

        SelectTool.Click += SelectTool_Click;
        RectTool.Click += RectTool_Click;
        TextTool.Click += TextTool_Click;
        QuadraticBCTool.Click += QuadraticBCTool_Click;
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


    private void RectTool_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        setButton(RectTool);
        toolManager.SetToolOption(ToolOption.Rect);
    }

    private void QuadraticBCTool_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        setButton(QuadraticBCTool);
        toolManager.SetToolOption(ToolOption.QBC);
    }

    private void setButton(Button button)
    {
        selectedButton.Classes.Clear();
        selectedButton = button;
        selectedButton.Classes.Add("Selected");
    }

    public void SetButton(ToolOption option)
    {
        if(option == ToolOption.Select)
        {
            setButton(SelectTool);
        }
        if (option == ToolOption.Rect)
        {
            setButton(RectTool);
        }
        if (option == ToolOption.Text)
        {
            setButton(TextTool);
        }
        if (option == ToolOption.QBC)
        {
            setButton(QuadraticBCTool);
        }
    }
}