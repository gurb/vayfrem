using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using vayfrem.models.enums;
using vayfrem.viewmodels;
using vayfrem.views.components;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Drawing.Text;
using vayfrem.models.lists;

namespace vayfrem.views.sections;

public partial class ToolOptionsView : UserControl
{
    private ToolOptionsViewModel ViewModel { get; set; }
    private Dictionary<string, List<Control>> Options { get; set; }

    public double ParentSize;

    List<double> rect_heights = new List<double>();
    List<double> text_heights = new List<double>();

    components.ColorPicker backgroundColorPicker;
    components.ColorPicker borderColorPicker;
    components.ColorPicker fontColorPicker;

    TextBlock rectOpacityValueText;
    TextBlock borderRadiusValueText;
    TextBlock borderThicknessValueText;


    components.Slider bgOpacitySlider;
    components.Slider borderRadiusSlider;
    components.Slider borderThicknessSlider;

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
            40,
            40,

            26,
            40,
            40,
            40,
        };


        // Background
        var row_background = RowOption("Background", null, null, rect_heights[0], true);
        var row_background_canvas = GetCanvas(rect_heights[0], row_background, Brushes.Gray);
        Options["Rect"].Add(row_background_canvas);

        backgroundColorPicker = new components.ColorPicker("bg");
        backgroundColorPicker.Background = new SolidColorBrush(ViewModel.RectToolDTO.BackgroundColorPicker.Color.ToColor());
        backgroundColorPicker.SelectedColor = ViewModel.RectToolDTO.BackgroundColorPicker.Color;
        backgroundColorPicker.Hex = ViewModel.RectToolDTO.BackgroundColorPicker.Color.ToHex();
        backgroundColorPicker.SetColorPickerDTO(ViewModel.RectToolDTO.BackgroundColorPicker);
        backgroundColorPicker.ValueChanged += RectBackgroundColor_ValueChanged;
        backgroundColorPicker.Margin = new Thickness(10, 5, 10, 5);
        var row_background_width = RowOption("Color", backgroundColorPicker, null, rect_heights[1]);
        var row_background_width_canvas = GetCanvas(rect_heights[1], row_background_width, Brushes.Gray);
        Options["Rect"].Add(row_background_width_canvas);


        bgOpacitySlider = new components.Slider();
        bgOpacitySlider.ValueChanged += BgOpactiyChanged_ValueChanged;
        bgOpacitySlider.Maximum = 255;
        bgOpacitySlider.Minimum = 0;

        var row_background_opacity = RowOption("Opacity", bgOpacitySlider, null, rect_heights[2]);
        var row_background_opacity_canvas = GetCanvas(rect_heights[2], row_background_opacity, Brushes.Gray);
        Options["Rect"].Add(row_background_opacity_canvas);


        // Border
        var row_border = RowOption("Border", null, null, rect_heights[3], true);
        var row_border_canvas = GetCanvas(rect_heights[3], row_border, Brushes.Gray);
        Options["Rect"].Add(row_border_canvas);

        borderColorPicker = new components.ColorPicker("borderColor");
        borderColorPicker.Background = new SolidColorBrush(ViewModel.RectToolDTO.BorderColorPicker.Color.ToColor());
        borderColorPicker.SetColorPickerDTO(ViewModel.RectToolDTO.BorderColorPicker);
        borderColorPicker.Hex = ViewModel.RectToolDTO.BorderColorPicker.Color.ToHex();
        borderColorPicker.ValueChanged += RectBorderColor_ValueChanged;
        borderColorPicker.Margin = new Thickness(10, 5, 10, 5);
        var row_border_color = RowOption("Color", borderColorPicker, null, rect_heights[4]);
        var row_border_color_canvas = GetCanvas(rect_heights[4], row_border_color, Brushes.Gray);
        Options["Rect"].Add(row_border_color_canvas);

        
        borderRadiusSlider = new components.Slider();
        borderRadiusSlider.ValueChanged += BorderRadiusChanged_ValueChanged;
        borderRadiusSlider.Value = (int)ViewModel.RectToolDTO.BorderRadius;
        borderRadiusSlider.Maximum = 100;
        borderRadiusSlider.Minimum = 0;
        var row_border_radius = RowOption("Radius", borderRadiusSlider, null, rect_heights[5]);
        var row_border_radius_canvas = GetCanvas(rect_heights[5], row_border_radius, Brushes.Gray);
        Options["Rect"].Add(row_border_radius_canvas);


        borderThicknessSlider = new components.Slider();
        borderThicknessSlider.ValueChanged += BorderThicknessChanged_ValueChanged;
        borderThicknessSlider.Value = (int)ViewModel.RectToolDTO.BorderThickness;
        borderThicknessSlider.Maximum = 100;
        borderThicknessSlider.Minimum = 0;

        var row_border_thickness = RowOption("Thickness", borderThicknessSlider, null, rect_heights[6]);
        var row_border_radius_thickness = GetCanvas(rect_heights[6], row_border_thickness, Brushes.Gray);
        Options["Rect"].Add(row_border_radius_thickness);
    }

    private void BgOpactiyChanged_ValueChanged()
    {
        ViewModel.RectToolDTO.Opacity = bgOpacitySlider.Value;
    }

    private void BorderRadiusChanged_ValueChanged()
    {
        ViewModel.RectToolDTO.BorderRadius = borderRadiusSlider.Value;
    }

    private void BorderThicknessChanged_ValueChanged()
    {
        ViewModel.RectToolDTO.BorderThickness = borderThicknessSlider.Value;
    }


    private void RadiusSlider_ValueChanged(object? sender, Avalonia.Controls.Primitives.RangeBaseValueChangedEventArgs e)
    {
        Avalonia.Controls.Slider slider = sender as Avalonia.Controls.Slider;

        ViewModel.RectToolDTO.BorderRadius = (int)slider.Value;
        borderRadiusValueText.Text = ViewModel.RectToolDTO.BorderRadius.ToString();
    }

    private void RectBackgroundColor_ValueChanged()
    {
        ViewModel.RectToolDTO.BackgroundColorPicker = new models.dtos.ColorPickerDTO
        {
            Color = backgroundColorPicker.SelectedColor,
            BarColor = backgroundColorPicker.SelectedBarColor,
            ColorSelectPosition = backgroundColorPicker.ColorSelectPosition,
            BarPosition = backgroundColorPicker.BarPosition,
        };
    }

    private void RectBorderColor_ValueChanged()
    {
        ViewModel.RectToolDTO.BorderColorPicker = new models.dtos.ColorPickerDTO
        {
            Color = borderColorPicker.SelectedColor,
            BarColor = borderColorPicker.SelectedBarColor,
            ColorSelectPosition = borderColorPicker.ColorSelectPosition,
            BarPosition = borderColorPicker.BarPosition,
        };
    }

    private void SetTextOption()
    {
        Options.Add("Text", new List<Control>());

        text_heights = new List<double>
        {
            26,
            40,
            40,

            40,
            26,
            50,
            50,
        };

        // Background
        var row_font = RowOption("Font", null, null, text_heights[0], true);
        var row_font_canvas = GetCanvas(text_heights[0], row_font, Brushes.Gray);
        Options["Text"].Add(row_font_canvas);

        
        fontColorPicker = new components.ColorPicker("fontColor");
        fontColorPicker.Background = new SolidColorBrush(ViewModel.TextToolDTO.FontColor.ToColor());
        fontColorPicker.Hex = ViewModel.TextToolDTO.FontColor.ToHex();
        fontColorPicker.ValueChanged += TextFontColor_ValueChanged;
        fontColorPicker.Margin = new Thickness(10, 5, 10, 5);
        var row_font_color = RowOption("Color", fontColorPicker, null, text_heights[1]);
        var row_font_color_canvas = GetCanvas(text_heights[1], row_font_color, Brushes.Gray);
        Options["Text"].Add(row_font_color_canvas);

        var fontFamilyComboBox = new ComboBox();
        // just for windows
        fontFamilyComboBox.ItemsSource = ListStorage.FontFamilies;
        fontFamilyComboBox.SelectionChanged += FontFamilyComboBox_SelectionChanged;
        fontFamilyComboBox.SelectedIndex = ViewModel.TextToolDTO.FontFamily != null ? ViewModel.TextToolDTO.SelectedFontFamilyIndex : 0;
        fontFamilyComboBox.Margin = new Thickness(10, 5, 10, 5);
        var row_font_family = RowOption("Font Family", fontFamilyComboBox, null, text_heights[2]);
        var row_font_family_canvas = GetCanvas(text_heights[1], row_font_family, Brushes.Gray);
        Options["Text"].Add(row_font_family_canvas);

        var fontSizeComboBox = new ComboBox();
        fontSizeComboBox.ItemsSource = ListStorage.FontSizes;
        fontSizeComboBox.SelectionChanged += FontSizeComboBox_SelectionChanged;
        fontSizeComboBox.SelectedIndex = ViewModel.TextToolDTO != null ? ViewModel.TextToolDTO.SelectedFontSizeIndex : 8;
        fontSizeComboBox.Margin = new Thickness(10, 5, 10, 5);
        var row_font_size = RowOption("Size", fontSizeComboBox, null, text_heights[3]);
        var row_font_size_canvas = GetCanvas(text_heights[1], row_font_size, Brushes.Gray);
        Options["Text"].Add(row_font_size_canvas);
    }

    private void FontFamilyComboBox_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        var fontFamilyComboBox = sender as ComboBox;

        ViewModel.TextToolDTO.FontFamily = (fontFamilyComboBox.SelectedValue as FontFamily).Name;
        ViewModel.TextToolDTO.SelectedFontFamilyIndex = fontFamilyComboBox.SelectedIndex;
    }

    private void FontSizeComboBox_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        var fontSizeComboBox = sender as ComboBox;

        ViewModel.TextToolDTO.FontSize = (int)fontSizeComboBox.SelectedValue;
        ViewModel.TextToolDTO.SelectedFontSizeIndex = fontSizeComboBox.SelectedIndex;
    }

    private void TextFontColor_ValueChanged()
    {
        ViewModel.TextToolDTO.FontColor = fontColorPicker.SelectedColor;
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
        if(!Options.ContainsKey("Text"))
            return;

        ToolOptionCanvas.Children.Clear();

        int counter = 0;

        foreach (var item in Options["Text"])
        {
            var height = rect_heights.GetRange(0, counter).Sum();

            Canvas.SetLeft(item, 0);
            Canvas.SetTop(item, height);

            ToolOptionCanvas.Children.Add(item);
            counter++;
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
            control.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch;
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