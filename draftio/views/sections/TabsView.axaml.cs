using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using draftio.viewmodels;

namespace draftio;

public partial class TabsView : UserControl
{
    TabViewModel ViewModel { get; set; }

    public TabsView()
    {
        ViewModel = App.GetService<TabViewModel>();
        DataContext = ViewModel;

        InitializeComponent();
    }



}