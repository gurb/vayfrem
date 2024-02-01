using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using vayfrem.models.objects;
using vayfrem.models.objects.components;
using vayfrem.viewmodels;

namespace vayfrem.views.sections;

public partial class SvgView : UserControl
{
    private readonly LayoutViewModel layoutViewModel;

    Dictionary<string, List<string>> svgFiles = new Dictionary<string, List<string>>(); 
    List<Canvas> svgNodes = new List<Canvas>();
    List<Avalonia.Svg.Svg> activeSvg = new List<Avalonia.Svg.Svg>();

    bool  isExist = false;

    Avalonia.Svg.Svg test;

    string? currentDir;

    int columnLen = 0;
    int rowLen = 0;

    double width;

    string svgDir;

    public SvgView()
    {
        layoutViewModel = App.GetService<LayoutViewModel>();

        InitializeComponent();

        SvgMenu.Background = Brushes.White;
        SvgMenu.SizeChanged += SvgMenu_SizeChanged;

        setStyle();

        GetFiles();

        set();

        svgComboBox.SelectionChanged += SvgComboBox_SelectionChanged;

        FolderButton.Click += FolderButton_Click;
    }

    private void FolderButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (!Directory.Exists(svgDir)) 
        {
            Directory.CreateDirectory(svgDir);
        }

        var psi = new System.Diagnostics.ProcessStartInfo()
        {
            FileName = svgDir,
            UseShellExecute = true
        };

        Process.Start(psi);
    }

    private void SvgComboBox_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        var combobox = sender as ComboBox;
            
        currentDir = (string)combobox.SelectedValue;

        SetCurrentDir();
    }

    private void setStyle()
    {
        svgMenuHeader.Background = new SolidColorBrush(Avalonia.Media.Color.FromArgb(255, 204, 208, 209));
        svgMenuFooter.Background = new SolidColorBrush(Avalonia.Media.Color.FromArgb(255, 204, 208, 209));
    }

    private void GetFiles()
    {
        string? executableLocation = System.IO.Path.GetDirectoryName(System.AppContext.BaseDirectory);
        string svgFolderLocation = System.IO.Path.Combine(executableLocation!, "svg");

        svgDir = svgFolderLocation;

        if (Directory.Exists(svgFolderLocation))
        {
            // yep
            
            //string svgLocation = Path.Combine(executableLocation!, "svg\\tiger.svg");
            //string[] fileEntries = Directory.GetFiles(svgFolderLocation);
            string[] dirEntries = Directory.GetDirectories(svgFolderLocation);
            List<string> dirNames = new List<string>();
            
            foreach (var dir in dirEntries)
            {
                string dirName = System.IO.Path.GetFileName(dir)!;
                svgFiles.Add(dirName, new List<string>());

                string dirLocation = System.IO.Path.Combine(svgFolderLocation, dir);

                string[] fileEntries = Directory.GetFiles(dirLocation);

                dirNames.Add(dirName);

                foreach (var file in fileEntries)
                {
                    svgFiles[dirName].Add(file);
                }
            }

            if(dirNames.Count() > 0)
            {
                currentDir = dirNames[0];
            }

            if (currentDir != null)
            {
                foreach (var file in svgFiles[currentDir])
                {
                    activeSvg.Add(GetSvg(file));
                }
            }

            svgComboBox.ItemsSource = dirNames;
            svgComboBox.SelectedIndex = 0;
        }
    }

    private void SetCurrentDir()
    {
        activeSvg = new List<Avalonia.Svg.Svg>();

        if (currentDir != null)
        {
            foreach (var file in svgFiles[currentDir])
            {
                activeSvg.Add(GetSvg(file));
            }
        }

        set();

        DrawCanvas();
    }

    private Avalonia.Svg.Svg GetSvg(string path)
    {
        Avalonia.Svg.Svg svg;
        Uri uri = new System.Uri(path);

        svg = new Avalonia.Svg.Svg(uri);
        svg.Path = path;
        svg.Width = 64;
        svg.Height = 64;

        return svg;
    }

    private Canvas SvgNode()
    {
        Canvas canvas = new Canvas();

        canvas.Width = 64;
        canvas.Height = 64;

        canvas.Background = Brushes.Transparent;

        return canvas;
    }

    private void DrawCanvas()
    {
        SvgMenu.Children.Clear();

        if (SvgMenu.Bounds.Width > 0)
        {
            columnLen = 0;
            rowLen = 0;

            if((69 * svgNodes.Count) <= (int)SvgMenu.Bounds.Width)
            {
                columnLen = svgNodes.Count;
                rowLen = 1;
            } 
            else
            {
                columnLen = (int)(SvgMenu.Bounds.Width / 69.0);

                if(columnLen == 0)
                {
                    columnLen = 1;
                }

                rowLen = svgNodes.Count / columnLen;


                if ((svgNodes.Count % columnLen) != 0)
                {
                    rowLen += 1;
                }
            }

            SvgMenu.Height = rowLen * 74;


            int counter = 0;
            for (global::System.Int32 y = 0; y < rowLen; y++)
            {
                for (global::System.Int32 x = 0; x < columnLen; x++)
                {
                    if(counter == svgNodes.Count)
                    {
                        return;
                    }

                    Canvas.SetLeft(svgNodes[counter], (x * 64 + x * 5) + 5);
                    Canvas.SetTop(svgNodes[counter], (y * 64 + y * 5) + 5);
                    SvgMenu.Children.Add(svgNodes[counter]);

                    counter++;
                }
            }
        }
    }

    private void SvgMenu_SizeChanged(object? sender, SizeChangedEventArgs e)
    {

        if(width != SvgMenu.Bounds.Width)
            DrawCanvas();

        width = SvgMenu.Bounds.Width;
    }

    private void set()
    {
        svgNodes.Clear();

        for (int i = 0; i < activeSvg.Count; i++)
        {
            Canvas canvas = SvgNode();
            canvas.Name = i.ToString();
            canvas.PointerPressed += Canvas_PointerPressed;
            canvas.Children.Add(activeSvg[i]);
            Canvas.SetLeft(canvas, 0);
            Canvas.SetTop(canvas, 0);
            svgNodes.Add(canvas);
        }
    }

    private void Canvas_PointerPressed(object? sender, Avalonia.Input.PointerPressedEventArgs e)
    {
        Canvas canvas = sender as Canvas;

        int index = Int32.Parse(canvas.Name);

        SvgObj svgObj = new SvgObj();
        svgObj.Path = activeSvg[index].Path;

        layoutViewModel.IsDrag = true;
        layoutViewModel.DragObject = svgObj;
        layoutViewModel.IsDragCompleted = false;
        layoutViewModel.Counter = 1;
    }
}