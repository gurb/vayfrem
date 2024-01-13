using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.VisualTree;

namespace draftio;

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
        NewProjectBtn.Click += NewProjectBtn_Click;
    }

    private void NewProjectBtn_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
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