using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using vayfrem.services;

namespace vayfrem;

public partial class AboutWindow : Window
{

    private readonly VersionControlManager versionControlManager;

    Grid grid;
    StackPanel contentStack;
    TextBlock content;
    TextBlock versionContent;


    int width_window = 250;
    int height_window = 150;

    public AboutWindow()
    {
        versionControlManager = App.GetService<VersionControlManager>();

        InitializeComponent();
        SetWindow();
    }

    private void SetWindow()
    {
        Title = "About Vayfrem";
        this.Width = width_window;
        this.Height = height_window;
        this.MinWidth = width_window;
        this.MinHeight = height_window;
        this.MaxWidth = width_window;
        this.MaxHeight = height_window;
        this.CanResize = false;
        this.ExtendClientAreaChromeHints = Avalonia.Platform.ExtendClientAreaChromeHints.NoChrome;
        this.ExtendClientAreaToDecorationsHint = false;

        SetElements();
    }

    private void SetElements()
    {
        grid = new Grid();
        grid.ColumnDefinitions = new ColumnDefinitions("*");
        grid.Margin = new Thickness(20);

        contentStack = new StackPanel();
        contentStack.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch;
        contentStack.VerticalAlignment = Avalonia.Layout.VerticalAlignment.Stretch;
        Grid.SetColumn(contentStack, 0);

        content = new TextBlock();
        content.Text = "Vayfrem is a wireframe tool. For more info vayfrem.org and gurbuz.itch.io/vayfrem. Developer Gürbüz Uğurgül[github.com/gurb]";
        content.TextWrapping = Avalonia.Media.TextWrapping.Wrap;
        content.TextAlignment = Avalonia.Media.TextAlignment.Center;
        content.FontSize = 12;

        versionContent = new TextBlock();
        versionContent.Text = "Vayfrem " + versionControlManager.Version;
        versionContent.TextWrapping = Avalonia.Media.TextWrapping.Wrap;
        versionContent.TextAlignment = Avalonia.Media.TextAlignment.Center;
        versionContent.FontWeight = Avalonia.Media.FontWeight.Medium;
        versionContent.FontSize = 12;

        contentStack.Children.Add(versionContent);
        contentStack.Children.Add(new TextBlock());
        contentStack.Children.Add(content);

        grid.Children.Add(contentStack);

        Content = grid;
    }
}