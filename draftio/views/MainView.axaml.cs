using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using draftio.models.dtos;
using draftio.services;
using draftio.viewmodels;

namespace draftio;

public partial class MainView : UserControl
{

   
    public MainViewModel ViewModel { get; private set; }

    public MainView()
    {
        ViewModel = App.GetService<MainViewModel>();
        DataContext = ViewModel;
        InitializeComponent();
    
        CheckProjectStatus();
    }


    public async void CheckProjectStatus()
    {
        VMResponse response = ViewModel.loadResponse;

        if(!response.Success)
        {
            await MessageBox.Show(this, "Error", response.Message!, MessageBox.MessageBoxButtons.Ok);
        }
        await MessageBox.Show(this, "Error", response.Message!, MessageBox.MessageBoxButtons.Ok);
    }
}