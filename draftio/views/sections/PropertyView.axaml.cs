using Avalonia.Controls.Templates;
using Avalonia.Metadata;
using draftio.models.structs;
using draftio.viewmodels;
using System.Collections.Generic;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using static System.Net.Mime.MediaTypeNames;
using draftio.views.components;
using Avalonia.Controls;

namespace draftio.views.sections
{
    public partial class PropertyView : UserControl
    {
        PropertyViewModel ViewModel { get; set; }

        draftio.views.components.DataGrid grid; 

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

            //ViewModel.RefreshDraw();
        }

        private void DataGrid_CellPointerPressed(object? sender, DataGridCellPointerPressedEventArgs e)
        {
            var cellValue = e.Cell.Content;
            DataGridRow row = e.Row;

            //var property = datagrid.ItemsSource.Cast<Property>().ElementAt(row.GetIndex());
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
                ViewModel.Properties = new ObservableCollection<Property>()
                {
                    new Property(ValueType.Name, ViewModel.ActiveObj.ObjectType.ToString()),
                    new Property(ValueType.X, ViewModel.ActiveObj.X),
                    new Property(ValueType.Y, ViewModel.ActiveObj.Y),
                    new Property(ValueType.Width, ViewModel.ActiveObj.Width),
                    new Property(ValueType.Height, ViewModel.ActiveObj.Height),
                    new Property(ValueType.Background, ViewModel.ActiveObj.BackgroundColor),
                    new Property(ValueType.Opacity, ViewModel.ActiveObj.Opacity),
                    new Property(ValueType.BorderColor, ViewModel.ActiveObj.BorderColor),
                    new Property(ValueType.BorderRadius, ViewModel.ActiveObj.BorderRadius),
                    new Property(ValueType.BorderThickness, ViewModel.ActiveObj.BorderThickness),
                    new Property(ValueType.Name, ViewModel.ActiveObj.ObjectType.ToString()),
                    new Property(ValueType.X, ViewModel.ActiveObj.X),
                    new Property(ValueType.Y, ViewModel.ActiveObj.Y),
                    new Property(ValueType.Width, ViewModel.ActiveObj.Width),
                    new Property(ValueType.Height, ViewModel.ActiveObj.Height),
                    new Property(ValueType.Background, ViewModel.ActiveObj.BackgroundColor),
                    new Property(ValueType.Opacity, ViewModel.ActiveObj.Opacity),
                    new Property(ValueType.BorderColor, ViewModel.ActiveObj.BorderColor),
                    new Property(ValueType.BorderRadius, ViewModel.ActiveObj.BorderRadius),
                    new Property(ValueType.BorderThickness, ViewModel.ActiveObj.BorderThickness),
                };
                grid.SetProperties(ViewModel.Properties.ToList());
            }

            if (ViewModel.ActiveObj!.ObjectType == models.enums.ObjectType.Text)
            {
                ViewModel.Properties = new ObservableCollection<Property>()
                {
                    new Property(ValueType.Name, ViewModel.ActiveObj.ObjectType.ToString()),
                    new Property(ValueType.X, ViewModel.ActiveObj.X),
                    new Property(ValueType.Y, ViewModel.ActiveObj.Y),
                    new Property(ValueType.Width, ViewModel.ActiveObj.Width),
                    new Property(ValueType.Height, ViewModel.ActiveObj.Height),
                    new Property(ValueType.Background, ViewModel.ActiveObj.BackgroundColor)
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
        Background,
        Opacity,
        BorderColor,
        BorderRadius,
        BorderThickness,
    }
}
