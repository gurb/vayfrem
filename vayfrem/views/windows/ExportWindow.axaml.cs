using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media.Imaging;
using Avalonia.Platform.Storage;
using Avalonia.VisualTree;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using vayfrem.services;

namespace vayfrem;

public partial class ExportWindow : Window
{
    private readonly ExportManager exportManager;
    private readonly TempStorage tempStorage;

    int width_window = 400;
    int height_window = 270;

    public ExportWindow()
    {
        exportManager = App.GetService<ExportManager>();
        tempStorage = App.GetService<TempStorage>();
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
        SetTempStorage();
    }

    private void SetTempStorage()
    {
        exportNameTxt.Text = tempStorage.GetTempData("exportName") != null ? tempStorage.GetTempData("exportName")!.ToString() : "";
        filePathTxt.Text = tempStorage.GetTempData("exportFilePath") != null ? tempStorage.GetTempData("exportFilePath")!.ToString() : "";
        fileTypeCb.SelectedIndex = tempStorage.GetTempData("selectedFileType") != null ? (int)tempStorage.GetTempData("selectedFileType")! : 0;
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
            filePathTxt.Text = result[0].Path.AbsolutePath.Replace("%20", " ");
            tempStorage.AddTempData("exportFilePath", filePathTxt.Text);
        }
    }

    private async void ExportBtn_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if(!exportManager.CurrentFileValid() && exportTypeCb.SelectedIndex == 0)
        {
            await MessageBox.Show(this, "Error", $"Current page not found", MessageBox.MessageBoxButtons.Ok);
            return;
        }

        if (!exportManager.ProjectValid() && exportTypeCb.SelectedIndex == 1)
        {
            await MessageBox.Show(this, "Error", $"Not found valid file for the project", MessageBox.MessageBoxButtons.Ok);
            return;
        }

        if (string.IsNullOrEmpty(exportNameTxt.Text))
        {
            await MessageBox.Show(this, "Error", $"File name cannot be empty", MessageBox.MessageBoxButtons.Ok);
            return;
        }

        if(string.IsNullOrEmpty(filePathTxt.Text))
        {
            await MessageBox.Show(this, "Error", $"Directory path is empty", MessageBox.MessageBoxButtons.Ok);
            return;
        }

        if(!System.IO.Directory.Exists(filePathTxt.Text))
        {
            await MessageBox.Show(this, "Error", $"Directory path not found", MessageBox.MessageBoxButtons.Ok);
            return;
        }

        if (fileTypeCb.SelectedValue == null) return;

        ComboBoxItem selectedItem = fileTypeCb.SelectedValue as ComboBoxItem;
        string content = selectedItem.Content.ToString();

        tempStorage.AddTempData("selectedFileType", fileTypeCb.SelectedIndex);
        tempStorage.AddTempData("exportName", exportNameTxt.Text);

        if (getExtension(content!) == "png")
        {
            ExportPNG();
        } 
        else if(getExtension(content!) == "pdf")
        {
            ExportPDF();
        }
        else if (getExtension(content!) == "html")
        {
            ExportHTML();
        }
    }

    private void ExportPNG()
    {
        if (exportTypeCb.SelectedValue == null) return;

        if (exportTypeCb.SelectedIndex == 0) // current file
        {
            RenderTargetBitmap bitmap = exportManager.GenerateCurrentPng();

            bitmap.Save(filePathTxt.Text + "/" + exportNameTxt.Text + ".png");
            bitmap.Dispose();
        } 
        else if (exportTypeCb.SelectedIndex == 1) // project
        {
            List<RenderTargetBitmap> bitmaps = exportManager.GenerateAllPagesPng();

            int fileCounter = 1;
            foreach (RenderTargetBitmap bitmap in bitmaps)
            {
                bitmap.Save(filePathTxt.Text + "/" + exportNameTxt.Text + "-" +  fileCounter.ToString() + ".png");
                bitmap.Dispose();
                fileCounter++;
            }
        }
    }

    private void ExportPDF()
    {
        if (exportTypeCb.SelectedValue == null) return;

        if (exportTypeCb.SelectedIndex == 0) // current file
        {
            exportManager.GenerateCurrentPdf(filePathTxt.Text + "/" + exportNameTxt.Text + ".pdf");
        }
        else if (exportTypeCb.SelectedIndex == 1) // project
        {
            exportManager.GenerateAllPdf(filePathTxt.Text + "/" + exportNameTxt.Text + ".pdf");
            //List<RenderTargetBitmap> bitmaps = exportManager.GenerateAllPagesPng();

            //int fileCounter = 1;
            //foreach (RenderTargetBitmap bitmap in bitmaps)
            //{
            //    bitmap.Save(filePathTxt.Text + "/" + exportNameTxt.Text + "-" + fileCounter.ToString() + ".png");
            //    bitmap.Dispose();
            //    fileCounter++;
            //}
        }
    }
    private void ExportHTML()
    {
        if (exportTypeCb.SelectedValue == null) return;

        if (exportTypeCb.SelectedIndex == 0) // current file
        {
            exportManager.GenerateCurrentHTML(filePathTxt.Text!, filePathTxt.Text + "/" + exportNameTxt.Text + ".html");
        }
        else if (exportTypeCb.SelectedIndex == 1) // project
        {
            exportManager.GenerateAllHTML(filePathTxt.Text + "/" + exportNameTxt.Text + ".html");
            //List<RenderTargetBitmap> bitmaps = exportManager.GenerateAllPagesPng();

            //int fileCounter = 1;
            //foreach (RenderTargetBitmap bitmap in bitmaps)
            //{
            //    bitmap.Save(filePathTxt.Text + "/" + exportNameTxt.Text + "-" + fileCounter.ToString() + ".png");
            //    bitmap.Dispose();
            //    fileCounter++;
            //}
        }
    }

    private string getExtension(string val)
    {
        List<string> subs = val.Split('.').ToList();

        return subs.Last();
    }
}