using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using vayfrem.services;

namespace vayfrem.views.sections;

public partial class TestView : UserControl
{
    private readonly MultipleUserService multipleUserService;

    public TestView()
    {
        multipleUserService = App.GetService<MultipleUserService>();

        InitializeComponent();

        Set();
    }

    private void Set()
    {
        ActivateButton.Click += ActivateButton_Click;
        ConnectButton.Click += ConnectButton_Click;
    }

    private void ConnectButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        multipleUserService.Connect(IpTxt.Text!, System.Int32.Parse(PortTxt.Text!));
    }

    private void ActivateButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        multipleUserService.Activate();

        HostInformation.Text = multipleUserService.Host;
    }
}