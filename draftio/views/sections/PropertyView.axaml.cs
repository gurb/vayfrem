using Avalonia.Controls;
using draftio.models.structs;
using draftio.viewmodels;
using System.Collections.ObjectModel;
using System.Linq;

namespace draftio.views.sections
{
    public partial class PropertyView : UserControl
    {
        PropertyViewModel ViewModel { get; set; }



        public PropertyView()
        {
            ViewModel = App.GetService<PropertyViewModel>();
            ViewModel.setProperty += SetProperties;

            DataContext = ViewModel;

            InitializeComponent();

            SetStyles();
        }

        private void SetStyles()
        {
            datagrid.CellPointerPressed += DataGrid_CellPointerPressed;
            datagrid.CellEditEnded += Datagrid_CellEditEnded;
        }

        private void Datagrid_CellEditEnded(object? sender, DataGridCellEditEndedEventArgs e)
        {

            DataGridRow row = e.Row;
            var property = datagrid.ItemsSource.Cast<Property>().ElementAt(row.GetIndex());

            if(property.Name == "X")
            {
                ViewModel.ActiveObj.X = System.Int32.Parse(property.Value.ToString());
            }

            ViewModel.RefreshDraw();
        }

        private void DataGrid_CellPointerPressed(object? sender, DataGridCellPointerPressedEventArgs e)
        {
            var cellValue = e.Cell.Content;
            DataGridRow row = e.Row;

            var property = datagrid.ItemsSource.Cast<Property>().ElementAt(row.GetIndex());
        }

        private void SetProperties()
        {
            if (ViewModel.ActiveObj == null)
            {
                ViewModel.Properties = new ObservableCollection<Property>()
                {
                    
                };
                return;
            }

            if (ViewModel.ActiveObj!.ObjectType == models.enums.ObjectType.Canvas)
            {
                ViewModel.Properties = new ObservableCollection<Property>()
                {
                    new Property("Name", ViewModel.ActiveObj.ObjectType.ToString()),
                    new Property("X", ViewModel.ActiveObj.X),
                    new Property("Y", ViewModel.ActiveObj.Y),
                    new Property("Width", ViewModel.ActiveObj.Width),
                    new Property("Height", ViewModel.ActiveObj.Height),
                    new Property("Background", ViewModel.ActiveObj.BackgroundColor),
                    new Property("Opacity", ViewModel.ActiveObj.Opacity),
                    new Property("Border Color", ViewModel.ActiveObj.BorderColor),
                    new Property("Border Radius", ViewModel.ActiveObj.BorderRadius),
                    new Property("Border Thickness", ViewModel.ActiveObj.BorderThickness),
                };
            }

            if (ViewModel.ActiveObj!.ObjectType == models.enums.ObjectType.Text)
            {
                ViewModel.Properties = new ObservableCollection<Property>()
                {
                    new Property("Name", ViewModel.ActiveObj.ObjectType.ToString()),
                    new Property("X", ViewModel.ActiveObj.X),
                    new Property("Y", ViewModel.ActiveObj.Y),
                    new Property("Width", ViewModel.ActiveObj.Width),
                    new Property("Height", ViewModel.ActiveObj.Height),
                    new Property("Background", ViewModel.ActiveObj.BackgroundColor)
                };
            }
        }



    }
}
