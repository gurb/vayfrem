using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using vayfrem.models.dtos;
using vayfrem.services;
using vayfrem.viewmodels;

namespace vayfrem;

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