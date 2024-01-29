using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Platform.Storage;
using Avalonia.VisualTree;
using System.Collections.Generic;
using System.Linq;

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

        SetClickEvents();
    }

    private void SetClickEvents()
    {
        selectBtn.Click += SelectBtn_Click;
        exportBtn.Click += ExportBtn_Click;
    }

    private async void SelectBtn_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var topLevel = TopLevel.GetTopLevel(this);

        FolderPickerOpenOptions openOptions = new FolderPickerOpenOptions
        {
            AllowMultiple = false, 
            Title = "Select Folder",
        };

        var result = await topLevel.StorageProvider.OpenFolderPickerAsync(openOptions);

        if(result.Count >= 1)
        {
            filePathTxt.Text = result[0].Path.AbsoluteUri;
        }
    }

    private void ExportBtn_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if(string.IsNullOrEmpty(filePathTxt.Text))
        {
            // file path not found
            return;
        }

        if(!System.IO.Directory.Exists(filePathTxt.Text))
        {
            // dir not found
            return;
        }

        if (fileTypeCb.SelectedValue == null) return;

        string selected = fileTypeCb.SelectedValue.ToString();

        if (getExtension(selected!) == "png")
        {
            // export as png
            ExportPNG();
        }
    }

    private void ExportPNG()
    {
        if (exportTypeCb.SelectedValue == null) return;

        // current file
        if (exportTypeCb.SelectedIndex == 0)
        {

        // project
        } else if (exportTypeCb.SelectedIndex == 1)
        {

        }
    }

    private string getExtension(string val)
    {
        List<string> subs = val.Split('.').ToList();

        return subs[2];
    }
}