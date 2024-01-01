using Avalonia.Controls;
using draftio.viewmodels;

namespace draftio.views.sections
{
    public partial class PropertyView : UserControl
    {
        PropertyViewModel ViewModel { get; set; }

        public PropertyView()
        {
            ViewModel = App.GetService<PropertyViewModel>();
            DataContext = ViewModel;

            InitializeComponent();

        }



    }
}
