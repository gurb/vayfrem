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
        SetNewProjectBtn();
    }


    private void SetNewProjectBtn()
    {
        NewPageBtn.Click += NewPageBtn_Click;
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
}