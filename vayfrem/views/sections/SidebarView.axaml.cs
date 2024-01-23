using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using vayfrem.services;
using vayfrem.viewmodels;

namespace vayfrem;

public partial class SidebarView : UserControl
{
    ToolOptionsViewModel toolOptionsViewModel { get; set; }
    ToolManager toolManager { get; set; }

    public SidebarView()
    {
        toolManager = App.GetService<ToolManager>();
        toolOptionsViewModel = App.GetService<ToolOptionsViewModel>();


        InitializeComponent();

        TabPanel.SelectedIndex = 2;
        TabPanel.SelectedIndex = 3;
        TabPanel.SelectedIndex = 0;
    }

    private void LoadToolOption(object sender, PointerPressedEventArgs args)
    {
        toolOptionsViewModel.SetToolOption(toolManager.SelectedToolOption);
    }
}