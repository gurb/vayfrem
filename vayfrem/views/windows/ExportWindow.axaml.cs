using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace vayfrem;

public partial class ExportWindow : Window
{
    int width_window = 400;
    int height_window = 270;

    public ExportWindow()
    {
        InitializeComponent();

        SetWindow();
    }

    private void SetWindow()
    {
        this.Height = height_window;
        this.MinWidth = width_window;
        this.MinHeight = height_window;
        this.MaxWidth = width_window;
        this.MaxHeight = height_window;
        this.CanResize = false;
        this.ExtendClientAreaChromeHints = Avalonia.Platform.ExtendClientAreaChromeHints.NoChrome;
        this.ExtendClientAreaToDecorationsHint = false;
    }

}