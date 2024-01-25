using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.VisualTree;

namespace vayfrem;

public partial class TopbarView : UserControl
{
    public TopbarView()
    {
        InitializeComponent();
        Init();
    }

    private void Init()
    {
        SetButtonsClick();
    }


    private void SetButtonsClick()
    {
        NewPageBtn.Click += NewPageBtn_Click;
        AboutBtn.Click += AboutBtn_Click;
    }
   
    private void NewPageBtn_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        NewProjectWindow window = new NewProjectWindow();
        window.WindowStartupLocation = WindowStartupLocation.CenterOwner;


        Window? parent = (Window?)this.GetVisualRoot();

        if(parent != null)
        {
            window.ShowDialog(parent);
        }
    }

    private void AboutBtn_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        AboutWindow window = new AboutWindow();
        window.WindowStartupLocation = WindowStartupLocation.CenterOwner;


        Window? parent = (Window?)this.GetVisualRoot();

        if (parent != null)
        {
            window.ShowDialog(parent);
        }
    }
}