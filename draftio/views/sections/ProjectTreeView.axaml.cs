using Avalonia.Controls;
using draftio.viewmodels;

namespace draftio.views.sections
{
    public partial class ProjectTreeView : UserControl
    {

        ProjectTreeViewModel ViewModel { get; set; }  
        public ProjectTreeView()
        {
            ViewModel = App.GetService<ProjectTreeViewModel>();
            DataContext = ViewModel;

            InitializeComponent();
        }


    }
}
