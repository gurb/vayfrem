using vayfrem.models.structs;
using vayfrem.viewmodels;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using Avalonia.Controls;
using vayfrem.models.dtos;
using Avalonia.Media;
using Avalonia;
using vayfrem.models.lists;
using vayfrem.models.objects;
using vayfrem.models.objects.components;
using System.Runtime.InteropServices.ComTypes;

namespace vayfrem.views.sections
{
    public partial class PropertyView : UserControl
    {
        PropertyViewModel ViewModel { get; set; }

        vayfrem.views.components.DataGrid grid;

        TextBox name_property;
        TextBox x_property;
        TextBox y_property;
        TextBox width_property;
        TextBox height_property;
        TextBox text_property;
        components.ColorPicker bg_color_property;
        components.ColorPicker border_color_property;

        components.Slider bg_opacity_property;
        components.Slider border_radius_property;
        components.Slider border_thickness_property;

        components.ColorPicker font_color_property;
        ComboBox font_family_property;
        ComboBox font_size_property;
        ComboBox text_alignment_property;
        ComboBox content_alignment_property;

        public PropertyView()
        {
            ViewModel = App.GetService<PropertyViewModel>();
            ViewModel.setProperty += SetProperties;

            DataContext = ViewModel;

            InitializeComponent();

            grid = new components.DataGrid();
            grid.SetProperties(ViewModel.Properties.ToList());


            this.Content = grid;

            SetStyles();
            InitProperties();
        }

        public void InitProperties()
        {
            name_property = new TextBox();
            name_property.IsReadOnly = true;
            name_property.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch;
            name_property.Margin = new Avalonia.Thickness(0);
            name_property.BorderThickness = new Avalonia.Thickness(0);

            x_property = new TextBox();
            x_property.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch;
            x_property.TextChanged += X_property_TextChanged;
            x_property.Margin = new Avalonia.Thickness(0);
            x_property.BorderThickness = new Avalonia.Thickness(0);

            y_property = new TextBox();
            y_property.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch;
            y_property.TextChanged += Y_property_TextChanged;
            y_property.Margin = new Avalonia.Thickness(0);
            y_property.BorderThickness = new Avalonia.Thickness(0);

            width_property = new TextBox();
            width_property.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch;
            width_property.TextChanged += Width_property_TextChanged;
            width_property.Margin = new Avalonia.Thickness(0);
            width_property.BorderThickness = new Avalonia.Thickness(0);

            height_property = new TextBox();
            height_property.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch;
            height_property.TextChanged += Height_property_TextChanged;
            height_property.Margin = new Avalonia.Thickness(0);
            height_property.BorderThickness = new Avalonia.Thickness(0);

            text_property = new TextBox();
            text_property.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch;
            text_property.TextChanged += Text_property_TextChanged;
            text_property.Margin = new Avalonia.Thickness(0);
            text_property.BorderThickness = new Avalonia.Thickness(0);

            bg_color_property = new components.ColorPicker("property-bg");
            bg_color_property.ValueChanged += RectBackgroundColor_ValueChanged;
            bg_color_property.Margin = new Thickness(0);

            border_color_property = new components.ColorPicker("borderColor");
            border_color_property.ValueChanged += RectBorderColor_ValueChanged;
            border_color_property.Margin = new Thickness(0);

            bg_opacity_property = new components.Slider();
            bg_opacity_property.ValueChanged += RectOpacityChanged_ValueChanged;
            bg_opacity_property.Maximum = 255;
            bg_opacity_property.Minimum = 0;

            border_radius_property = new components.Slider();
            border_radius_property.ValueChanged += BorderRadiusChanged_ValueChanged;
            border_radius_property.Maximum = 100;
            border_radius_property.Minimum = 0;

            border_thickness_property = new components.Slider();
            border_thickness_property.ValueChanged += BorderThicknessChanged_ValueChanged;
            border_thickness_property.Maximum = 100;
            border_thickness_property.Minimum = 0;

            font_color_property = new components.ColorPicker("fontColor");
            font_color_property.ValueChanged += FontColor_ValueChanged;

            font_family_property = new ComboBox();
            font_family_property.ItemsSource = ListStorage.FontFamilies;
            font_family_property.SelectionChanged += FontFamilyComboBox_SelectionChanged;
            font_family_property.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch;
            font_family_property.BorderThickness = new Thickness(0);

            font_size_property = new ComboBox();
            font_size_property.ItemsSource = ListStorage.FontSizes;
            font_size_property.SelectionChanged += FontSizeComboBox_SelectionChanged;
            font_size_property.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch;
            font_size_property.BorderThickness = new Thickness(0);

            text_alignment_property = new ComboBox();
            text_alignment_property.ItemsSource = ListStorage.TextAlignments;
            text_alignment_property.SelectionChanged += TextAlignmentComboBox_SelectionChanged;
            text_alignment_property.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch;
            text_alignment_property.BorderThickness = new Thickness(0);

            content_alignment_property = new ComboBox();
            content_alignment_property.ItemsSource = ListStorage.ContentAlignments;
            content_alignment_property.SelectionChanged += ContentAlignmentComboBox_SelectionChanged;
            content_alignment_property.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch;
            content_alignment_property.BorderThickness = new Thickness(0);
        }

        private async void X_property_TextChanged(object? sender, TextChangedEventArgs e)
        {
            if (sender == null) return;
            TextBox textBox = (TextBox)sender;

            if (String.IsNullOrEmpty(textBox.Text)) return;

            if (!textBox.Text.All(char.IsDigit))
            {
                await MessageBox.Show(this, "Error", $"Input cannot be {textBox.Text}", MessageBox.MessageBoxButtons.Ok);
                return;
            }

            if (ViewModel.ActiveObj != null)
            {
                ViewModel.ActiveObj.X = Int32.Parse(textBox.Text);
                ViewModel.RefreshDraw();
            }
        }

        private async void Y_property_TextChanged(object? sender, TextChangedEventArgs e)
        {
            if (sender == null) return;
            TextBox textBox = (TextBox)sender;

            if (String.IsNullOrEmpty(textBox.Text)) return;

            if (!textBox.Text.All(char.IsDigit))
            {
                await MessageBox.Show(this, "Error", $"Input cannot be {textBox.Text}", MessageBox.MessageBoxButtons.Ok);
                return;
            }

            if (ViewModel.ActiveObj != null)
            {
                ViewModel.ActiveObj.Y = Int32.Parse(textBox.Text);
                ViewModel.RefreshDraw();
            }
        }

        private async void Width_property_TextChanged(object? sender, TextChangedEventArgs e)
        {
            if (sender == null) return;
            TextBox textBox = (TextBox)sender;

            if (String.IsNullOrEmpty(textBox.Text)) return;

            if (!textBox.Text.All(char.IsDigit))
            {
                await MessageBox.Show(this, "Error", $"Input cannot be {textBox.Text}", MessageBox.MessageBoxButtons.Ok);
                return;
            }

            if (ViewModel.ActiveObj != null)
            {
                ViewModel.ActiveObj.Width = Int32.Parse(textBox.Text);
                ViewModel.RefreshDraw();
            }
        }

        private async void Height_property_TextChanged(object? sender, TextChangedEventArgs e)
        {
            if (sender == null) return;
            TextBox textBox = (TextBox)sender;

            if (String.IsNullOrEmpty(textBox.Text)) return;

            if (!textBox.Text.All(char.IsDigit))
            {
                await MessageBox.Show(this, "Error", $"Input cannot be {textBox.Text}", MessageBox.MessageBoxButtons.Ok);
                return;
            }

            if (ViewModel.ActiveObj != null)
            {
                ViewModel.ActiveObj.Height = Int32.Parse(textBox.Text);
                ViewModel.RefreshDraw();
            }
        }


        private async void Text_property_TextChanged(object? sender, TextChangedEventArgs e)
        {
            if (sender == null) return;
            TextBox textBox = (TextBox)sender;

            if (ViewModel.ActiveObj != null)
            {
                if(ViewModel.ActiveObj.ObjectType == models.enums.ObjectType.Text)
                {
                    TextObj textObj = (TextObj)ViewModel.ActiveObj;
                    textObj.Text = textBox.Text;
                }

                if (ViewModel.ActiveObj.ObjectType == models.enums.ObjectType.Button)
                {
                    ButtonObj buttonObj = (ButtonObj)ViewModel.ActiveObj;
                    buttonObj.Text = textBox.Text;
                }

                ViewModel.RefreshDraw();
            }
        }

        
        private void RectBackgroundColor_ValueChanged()
        {
            if (ViewModel.ActiveObj != null)
            {
                ViewModel.ActiveObj!.BackgroundColor = bg_color_property.SelectedColor;
                ViewModel.RefreshDraw();
            }
        }

        private void RectBorderColor_ValueChanged()
        {
            if (ViewModel.ActiveObj != null)
            {
                ViewModel.ActiveObj!.BorderColor = border_color_property.SelectedColor;
                ViewModel.RefreshDraw();
            }
        }

        private void RectOpacityChanged_ValueChanged()
        {
            if (ViewModel.ActiveObj != null)
            {
                ViewModel.ActiveObj!.Opacity = bg_opacity_property.Value;
                ViewModel.RefreshDraw();
            }
        }

        private void BorderRadiusChanged_ValueChanged()
        {
            if (ViewModel.ActiveObj != null)
            { 
                ViewModel.ActiveObj!.BorderRadius = border_radius_property.Value;
                ViewModel.RefreshDraw();
            }
        }

        private void BorderThicknessChanged_ValueChanged()
        {
            if (ViewModel.ActiveObj != null)
            {
                ViewModel.ActiveObj!.BorderThickness = border_thickness_property.Value;
                ViewModel.RefreshDraw();
            }
        }
        private void FontColor_ValueChanged()
        {
            if(ViewModel.ActiveObj == null) {
                return;
            }

            if (ViewModel.ActiveObj!.ObjectType == models.enums.ObjectType.Text)
            {
                TextObj textObj = (TextObj)ViewModel.ActiveObj;
                textObj.FontColor = font_color_property.SelectedColor;
            }
            if (ViewModel.ActiveObj!.ObjectType == models.enums.ObjectType.Button)
            {
                ButtonObj buttonObj = (ButtonObj)ViewModel.ActiveObj;
                buttonObj.FontColor = font_color_property.SelectedColor;
            }

            ViewModel.RefreshDraw();
        }

        private void FontFamilyComboBox_SelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            if (ViewModel.ActiveObj == null)
            {
                return;
            }

            var fontFamilyComboBox = sender as ComboBox;

            if(ViewModel.ActiveObj!.ObjectType == models.enums.ObjectType.Text)
            {
                TextObj textObj = (TextObj)ViewModel.ActiveObj;
                textObj.FontFamily = (fontFamilyComboBox.SelectedValue as FontFamily).Name;
            }
            if(ViewModel.ActiveObj!.ObjectType == models.enums.ObjectType.Button)
            {
                ButtonObj buttonObj = (ButtonObj)ViewModel.ActiveObj;
                buttonObj.FontFamily = (fontFamilyComboBox.SelectedValue as FontFamily).Name;
            }

            ViewModel.RefreshDraw();
        }

        private void FontSizeComboBox_SelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            var fontSizeComboBox = sender as ComboBox;

            if (ViewModel.ActiveObj!.ObjectType == models.enums.ObjectType.Text)
            {
                TextObj textObj = (TextObj)ViewModel.ActiveObj;
                textObj.FontSize = (int)fontSizeComboBox.SelectedValue;
            }
            if (ViewModel.ActiveObj!.ObjectType == models.enums.ObjectType.Button)
            {
                ButtonObj buttonObj = (ButtonObj)ViewModel.ActiveObj;
                buttonObj.FontSize = (int)fontSizeComboBox.SelectedValue;
            }
            ViewModel.RefreshDraw();
        }

        private void TextAlignmentComboBox_SelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            var textAlignmentComboBox = sender as ComboBox;

            if (ViewModel.ActiveObj!.ObjectType == models.enums.ObjectType.Button)
            {
                ButtonObj buttonObj = (ButtonObj)ViewModel.ActiveObj;
                buttonObj.TextAlignment = (models.enums.TextAlignment)textAlignmentComboBox.SelectedValue;
            }
            ViewModel.RefreshDraw();
        }

        private void ContentAlignmentComboBox_SelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            var textAlignmentComboBox = sender as ComboBox;

            if (ViewModel.ActiveObj!.ObjectType == models.enums.ObjectType.Text)
            {
                TextObj textObj = (TextObj)ViewModel.ActiveObj;
                textObj.ContentAlignment = (models.enums.ContentAlignment)textAlignmentComboBox.SelectedValue;
            }
            
            ViewModel.RefreshDraw();
        }


        private void SetStyles()
        {
            //datagrid.CellPointerPressed += DataGrid_CellPointerPressed;
            //datagrid.CellEditEnded += Datagrid_CellEditEnded;
        }

        private void Datagrid_CellEditEnded(object? sender, DataGridCellEditEndedEventArgs e)
        {

            //DataGridRow row = e.Row;
            //var property = datagrid.ItemsSource.Cast<Property>().ElementAt(row.GetIndex());

            //if(property.Name == "X")
            //{
            //    ViewModel.ActiveObj.X = System.Int32.Parse(property.Value.ToString());
            //}

            
        }

        private void DataGrid_CellPointerPressed(object? sender, DataGridCellPointerPressedEventArgs e)
        {
            var cellValue = e.Cell.Content;
            DataGridRow row = e.Row;

            //var property = datagrid.ItemsSource.Cast<Property>().ElementAt(row.GetIndex());
        }

        private void SetValues()
        {
            // general properties
            name_property.Text = ViewModel.ActiveObj.ObjectType.ToString();
            x_property.Text = ViewModel.ActiveObj.X.ToString();
            y_property.Text = ViewModel.ActiveObj.Y.ToString();
            width_property.Text = ViewModel.ActiveObj.Width.ToString();
            height_property.Text = ViewModel.ActiveObj.Height.ToString();

            if (ViewModel.ActiveObj!.ObjectType == models.enums.ObjectType.Canvas)
            {
                bg_color_property.Background = new SolidColorBrush(ViewModel.ActiveObj.BackgroundColor.ToColor());
                bg_color_property.Hex = ViewModel.ActiveObj.BackgroundColor.ToHex();
                bg_color_property.SetColorPickerDTO(
                    new ColorPickerDTO
                    {
                        Color = ViewModel.ActiveObj.BackgroundColor,
                    }    
                );

                border_color_property.Background = new SolidColorBrush(ViewModel.ActiveObj.BorderColor.ToColor());

                bg_opacity_property.Value = (int)ViewModel.ActiveObj.Opacity;
                border_radius_property.Value = (int)ViewModel.ActiveObj.BorderRadius;
                border_thickness_property.Value = (int)ViewModel.ActiveObj.BorderThickness;
            }

            if(ViewModel.ActiveObj!.ObjectType == models.enums.ObjectType.Text)
            {
                TextObj textObj = (TextObj)ViewModel.ActiveObj;

                text_property.Text = textObj.Text;
                
                var contentAlignment = ListStorage.ContentAlignments.FirstOrDefault(x => x == textObj.ContentAlignment);
                content_alignment_property.SelectedIndex = ListStorage.ContentAlignments.IndexOf(contentAlignment);

                font_color_property.Background = new SolidColorBrush(textObj.FontColor.ToColor());

                var fontFamily = ListStorage.FontFamilies.FirstOrDefault(x => x.Name == textObj.FontFamily);
                if(fontFamily != null)
                {
                    font_family_property.SelectedIndex = ListStorage.FontFamilies.IndexOf(fontFamily);
                }

                int? fontSize = ListStorage.FontSizes.FirstOrDefault(x => x == textObj.FontSize);
                if (fontSize != null)
                {
                    font_size_property.SelectedIndex = ListStorage.FontSizes.IndexOf(fontSize.Value);
                }
            }

            if (ViewModel.ActiveObj!.ObjectType == models.enums.ObjectType.Button)
            {
                ButtonObj buttonObj = (ButtonObj)ViewModel.ActiveObj;

                text_property.Text = buttonObj.Text;

                var textAlignment = ListStorage.TextAlignments.FirstOrDefault(x => x == buttonObj.TextAlignment);
                text_alignment_property.SelectedIndex = ListStorage.TextAlignments.IndexOf(textAlignment);

                font_color_property.Background = new SolidColorBrush(buttonObj.FontColor.ToColor());

                var fontFamily = ListStorage.FontFamilies.FirstOrDefault(x => x.Name == buttonObj.FontFamily);
                if (fontFamily != null)
                {
                    font_family_property.SelectedIndex = ListStorage.FontFamilies.IndexOf(fontFamily);
                }

                int? fontSize = ListStorage.FontSizes.FirstOrDefault(x => x == buttonObj.FontSize);
                if (fontSize != null)
                {
                    font_size_property.SelectedIndex = ListStorage.FontSizes.IndexOf(fontSize.Value);
                }

                bg_color_property.Background = new SolidColorBrush(ViewModel.ActiveObj.BackgroundColor.ToColor());
                bg_color_property.Hex = ViewModel.ActiveObj.BackgroundColor.ToHex();
                bg_color_property.SetColorPickerDTO(
                    new ColorPickerDTO
                    {
                        Color = ViewModel.ActiveObj.BackgroundColor,
                    }
                );

                border_color_property.Background = new SolidColorBrush(ViewModel.ActiveObj.BorderColor.ToColor());

                bg_opacity_property.Value = (int)ViewModel.ActiveObj.Opacity;
                border_radius_property.Value = (int)ViewModel.ActiveObj.BorderRadius;
                border_thickness_property.Value = (int)ViewModel.ActiveObj.BorderThickness;
            }
        }

        private void SetProperties()
        {
            if (ViewModel.ActiveObj == null)
            {
                ViewModel.Properties = new ObservableCollection<Property>()
                {
                    
                };
                grid.SetProperties(ViewModel.Properties.ToList());
                return;
            }

            if (ViewModel.ActiveObj!.ObjectType == models.enums.ObjectType.Canvas)
            {
                SetValues();
                ViewModel.Properties = new ObservableCollection<Property>()
                {
                    new Property(ValueType.Name, name_property),
                    new Property(ValueType.X, x_property),
                    new Property(ValueType.Y, y_property),
                    new Property(ValueType.Width, width_property),
                    new Property(ValueType.Height, height_property),
                    new Property(ValueType.Background, bg_color_property),
                    new Property(ValueType.Opacity, bg_opacity_property),
                    new Property(ValueType.BorderColor, border_color_property),
                    new Property(ValueType.BorderRadius, border_radius_property),
                    new Property(ValueType.BorderThickness, border_thickness_property),
                };
                grid.SetProperties(ViewModel.Properties.ToList());
            }

            if (ViewModel.ActiveObj!.ObjectType == models.enums.ObjectType.Text)
            {
                SetValues();
                ViewModel.Properties = new ObservableCollection<Property>()
                {
                    new Property(ValueType.Name, name_property),
                    new Property(ValueType.X, x_property),
                    new Property(ValueType.Y, y_property),
                    new Property(ValueType.Width, width_property),
                    new Property(ValueType.Height, height_property),
                    new Property(ValueType.Text, text_property),
                    new Property(ValueType.ContentAlignment, content_alignment_property),
                    new Property(ValueType.FontColor, font_color_property),
                    new Property(ValueType.FontFamily, font_family_property),
                    new Property(ValueType.FontSize, font_size_property),
                };
                grid.SetProperties(ViewModel.Properties.ToList());
            }

            if(ViewModel.ActiveObj!.ObjectType == models.enums.ObjectType.Button)
            {
                SetValues();
                ViewModel.Properties = new ObservableCollection<Property>()
                {
                    new Property(ValueType.Name, name_property),
                    new Property(ValueType.X, x_property),
                    new Property(ValueType.Y, y_property),
                    new Property(ValueType.Width, width_property),
                    new Property(ValueType.Height, height_property),
                    new Property(ValueType.Text, text_property),
                    new Property(ValueType.TextAlignment, text_alignment_property),
                    new Property(ValueType.FontColor, font_color_property),
                    new Property(ValueType.FontFamily, font_family_property),
                    new Property(ValueType.FontSize, font_size_property),
                    new Property(ValueType.Background, bg_color_property),
                    new Property(ValueType.Opacity, bg_opacity_property),
                    new Property(ValueType.BorderColor, border_color_property),
                    new Property(ValueType.BorderRadius, border_radius_property),
                    new Property(ValueType.BorderThickness, border_thickness_property),
                };
                grid.SetProperties(ViewModel.Properties.ToList());
            }
        }
    }

    public enum ValueType
    {
        Name,
        X,
        Y,
        Width,
        Height,
        Text,
        TextAlignment,
        ContentAlignment,
        Background,
        Opacity,
        BorderColor,
        BorderRadius,
        BorderThickness,
        FontColor,
        FontFamily,
        FontSize,
    }
}
