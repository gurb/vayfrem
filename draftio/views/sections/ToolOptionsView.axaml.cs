using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using draftio.models.enums;
using draftio.viewmodels;
using draftio.views.components;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;

namespace draftio.views.sections;

public partial class ToolOptionsView : UserControl
{
    private ToolOptionsViewModel ViewModel { get; set; }
    private Dictionary<string, List<Control>> Options { get; set; }

    public double ParentSize;

    List<double> rect_heights = new List<double>();
    List<double> text_heights = new List<double>();

    components.ColorPicker backgroundColorPicker;
    components.ColorPicker borderColorPicker;
    TextBlock rectOpacityValueText;
    TextBlock borderRadiusValueText;
    TextBlock borderThicknessValueText;

    bool isLoaded;
    public ToolOptionsView()
    {
        ViewModel = App.GetService<ToolOptionsViewModel>();
        DataContext = ViewModel;
        ViewModel.drawToolOptionDelegate += Draw;

        Options = new Dictionary<string, List<Control>>();

        InitializeComponent();

        ToolOptionCanvas.SizeChanged += ToolOptionCanvas_SizeChanged;

        setStyle();
    }

    private void ToolOptionCanvas_SizeChanged(object? sender, SizeChangedEventArgs e)
    {
        Draw();
    }

    private void setStyle()
    {
        toolOptionHeader.Background = new SolidColorBrush(Avalonia.Media.Color.FromArgb(255, 204, 208, 209));
        toolOptionFooter.Background = new SolidColorBrush(Avalonia.Media.Color.FromArgb(255, 204, 208, 209));

        Draw();
    }

    private void Draw()
    {
        Options.Clear();

        ToolOptionCanvas.Children.Clear();

        SetRectOption();
        SetTextOption();

        switch (ViewModel.ToolOption)
        {
            case ToolOption.Select:
                break;
            case ToolOption.Rect:
                DrawRectOption();
                break;
            case ToolOption.Text:
                DrawTextOption();
                break;
            default:
                break;
        }
    }

    private void SetRectOption()
    {
        Options.Add("Rect", new List<Control>());

        rect_heights = new List<double>
        {
            26,
            26,
            50,

            26,
            26,
            50,
            50,
        };


        // Background
        var row_background = RowOption("Background", null, null, rect_heights[0], true);
        var row_background_canvas = GetCanvas(rect_heights[0], row_background, Brushes.Gray);
        Options["Rect"].Add(row_background_canvas);

        backgroundColorPicker = new components.ColorPicker();
        backgroundColorPicker.Background = new SolidColorBrush(ViewModel.RectToolDTO.Background);
        backgroundColorPicker.ValueChanged += RectBackgroundColor_ValueChanged;
        backgroundColorPicker.Margin = new Thickness(10, 5, 10, 5);
        var row_background_width = RowOption("Color", backgroundColorPicker, null, rect_heights[1]);
        var row_background_width_canvas = GetCanvas(rect_heights[1], row_background_width, Brushes.Gray);
        Options["Rect"].Add(row_background_width_canvas);


        Slider opacitySlider = new Slider();
        opacitySlider.Margin = new Thickness(5, 0, 10, 0);
        opacitySlider.Value = ViewModel.RectToolDTO.BorderThickness;
        opacitySlider.ValueChanged += OpacitySlider_ValueChanged;

        rectOpacityValueText = new TextBlock();
        rectOpacityValueText.Text = ViewModel.RectToolDTO.BorderRadius.ToString();
        rectOpacityValueText.Margin = new Thickness(5);
        rectOpacityValueText.VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center;
        var row_background_opacity = RowOption("Opacity", opacitySlider, rectOpacityValueText, rect_heights[2]);
        var row_background_opacity_canvas = GetCanvas(rect_heights[2], row_background_opacity, Brushes.Gray);
        Options["Rect"].Add(row_background_opacity_canvas);


        // Border
        var row_border = RowOption("Border", null, null, rect_heights[3], true);
        var row_border_canvas = GetCanvas(rect_heights[3], row_border, Brushes.Gray);
        Options["Rect"].Add(row_border_canvas);

        borderColorPicker = new components.ColorPicker();
        borderColorPicker.Background = new SolidColorBrush(ViewModel.RectToolDTO.BorderColor);
        borderColorPicker.ValueChanged += RectBorderColor_ValueChanged;
        borderColorPicker.Margin = new Thickness(10, 5, 10, 5);
        var row_border_color = RowOption("Color", borderColorPicker, null, rect_heights[4]);
        var row_border_color_canvas = GetCanvas(rect_heights[4], row_border_color, Brushes.Gray);
        Options["Rect"].Add(row_border_color_canvas);

        Slider radiusSlider = new Slider();
        radiusSlider.Margin = new Thickness(5, 0, 10, 0);
        radiusSlider.Value = ViewModel.RectToolDTO.BorderRadius;
        radiusSlider.ValueChanged += RadiusSlider_ValueChanged;

        borderRadiusValueText = new TextBlock();
        borderRadiusValueText.Text = ViewModel.RectToolDTO.BorderRadius.ToString();
        borderRadiusValueText.Margin = new Thickness(5);
        borderRadiusValueText.VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center;
        var row_border_radius = RowOption("Radius", radiusSlider, borderRadiusValueText, rect_heights[5]);
        var row_border_radius_canvas = GetCanvas(rect_heights[5], row_border_radius, Brushes.Gray);
        Options["Rect"].Add(row_border_radius_canvas);

        Slider thicknessSlider = new Slider();
        thicknessSlider.Margin = new Thickness(5, 0, 10, 0);
        thicknessSlider.Value = ViewModel.RectToolDTO.BorderThickness;
        thicknessSlider.ValueChanged += ThicknessSlider_ValueChanged;

        borderThicknessValueText = new TextBlock();
        borderThicknessValueText.Text = ViewModel.RectToolDTO.BorderRadius.ToString();
        borderThicknessValueText.Margin = new Thickness(5);
        borderThicknessValueText.VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center;
        var row_border_thickness = RowOption("Thickness", thicknessSlider, borderThicknessValueText, rect_heights[6]);
        var row_border_radius_thickness = GetCanvas(rect_heights[6], row_border_thickness, Brushes.Gray);
        Options["Rect"].Add(row_border_radius_thickness);
    }

    private void OpacitySlider_ValueChanged(object? sender, Avalonia.Controls.Primitives.RangeBaseValueChangedEventArgs e)
    {
        Slider slider = sender as Slider;

        ViewModel.RectToolDTO.Opacity = (int)slider.Value;
        rectOpacityValueText.Text = ViewModel.RectToolDTO.Opacity.ToString();
    }

    private void ThicknessSlider_ValueChanged(object? sender, Avalonia.Controls.Primitives.RangeBaseValueChangedEventArgs e)
    {
        Slider slider = sender as Slider;

        ViewModel.RectToolDTO.BorderThickness = (int)slider.Value;
        borderThicknessValueText.Text = ViewModel.RectToolDTO.BorderThickness.ToString();
    }

    private void RadiusSlider_ValueChanged(object? sender, Avalonia.Controls.Primitives.RangeBaseValueChangedEventArgs e)
    {
        Slider slider = sender as Slider;

        ViewModel.RectToolDTO.BorderRadius = (int)slider.Value;
        borderRadiusValueText.Text = ViewModel.RectToolDTO.BorderRadius.ToString();
    }

    private void RectBackgroundColor_ValueChanged()
    {
        ViewModel.RectToolDTO.Background = backgroundColorPicker.SelectedColor;
    }

    private void RectBorderColor_ValueChanged()
    {
        ViewModel.RectToolDTO.BorderColor = borderColorPicker.SelectedColor;
    }

    private void SetTextOption()
    {
        Options.Add("Text", new List<Control>());

        text_heights = new List<double>
        {
            26,
            26,
            50,

            26,
            26,
            50,
            50,
        };

        // Background
        var row_font = RowOption("Font", null, null, text_heights[0], true);
        var row_font_canvas = GetCanvas(rect_heights[0], row_font, Brushes.Gray);
        Options["Text"].Add(row_font_canvas);

        var fontFamilyComboBox = new ComboBox();
        //fontFamilyComboBox.Items = FontManager.Current.GetInstalledFontFamilyNames()
        //        .Select(x => new FontFamily(x))
        //        .OrderBy(x => x.Name);

        backgroundColorPicker = new components.ColorPicker();
        backgroundColorPicker.Background = new SolidColorBrush(ViewModel.RectToolDTO.Background);
        backgroundColorPicker.ValueChanged += RectBackgroundColor_ValueChanged;
        backgroundColorPicker.Margin = new Thickness(10, 5, 10, 5);
        var row_background_width = RowOption("Color", backgroundColorPicker, null, rect_heights[1]);
        var row_background_width_canvas = GetCanvas(rect_heights[1], row_background_width, Brushes.Gray);
        Options["Text"].Add(row_background_width_canvas);
    }

    private void DrawRectOption()
    {
        if (!Options.ContainsKey("Rect"))
            return;

        ToolOptionCanvas.Height = rect_heights.Sum();

        int counter = 0;

        foreach (var item in Options["Rect"])
        {
            var height = rect_heights.GetRange(0, counter).Sum();

            Canvas.SetLeft(item, 0);
            Canvas.SetTop(item, height);

            ToolOptionCanvas.Children.Add(item);
            counter++;
        }

    }

    private void DrawTextOption()
    {
        ToolOptionCanvas.Children.Clear();

        foreach (var item in Options)
        {

        }
    }

    private Border GetCanvas(double height, Control control, IBrush backgroundColor)
    {
        Border border = new Border();
        border.Height = height;
        border.BorderThickness = new Thickness(0);
        
        border.Background = backgroundColor;

        Canvas canvas = new Canvas();
        canvas.Height = height;
        

        Canvas.SetLeft(control, 0);
        Canvas.SetTop(control, 0);
        Canvas.SetRight(control, 0);


        canvas.Children.Add(control);
        border.Child = canvas;

        return border;
    }


    private Grid RowOption(string optionName, Control? control, Control? resultControl, double height, bool? isHeader = false)
    {
        Grid grid = new Grid();
        
        grid.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch;
        grid.VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center;
        grid.Height = height;
        grid.Background = isHeader == true  ? Avalonia.Media.Brushes.LightGray : Avalonia.Media.Brushes.Transparent;
        grid.Width = ToolOptionCanvas.Bounds.Width;
        if(resultControl != null)
        {
            grid.ColumnDefinitions = new ColumnDefinitions("*, *, 50");
        }
        else 
        { 
            grid.ColumnDefinitions = new ColumnDefinitions("*, *");
        }


        TextBlock optionNameBlock = new TextBlock();
        optionNameBlock.Text = optionName;
        optionNameBlock.Margin = new Thickness(5, 0, 5, 0);
        optionNameBlock.VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center;

        grid.Children.Add(optionNameBlock);

        Grid.SetColumn(optionNameBlock, 0);

        if(control != null)
        {
            grid.Children.Add(control);
            Grid.SetColumn(control, 1);
        }

        if (resultControl != null)
        {
            grid.Children.Add(resultControl);
            Grid.SetColumn(resultControl, 2);
        }

        return grid;
    }



}