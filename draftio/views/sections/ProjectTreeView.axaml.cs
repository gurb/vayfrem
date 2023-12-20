using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using draftio.models;
using draftio.models.enums;
using draftio.viewmodels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.Metrics;
using System.Drawing;
using System.Linq;

namespace draftio.views.sections
{
    public partial class ProjectTreeView : UserControl
    {
        ProjectTreeViewModel ViewModel { get; set; }
        
        public ProjectTreeView()
        {
            ViewModel = App.GetService<ProjectTreeViewModel>();
            DataContext = ViewModel;

            InitializeComponent();

            AddFileButton.Click += AddFileButton_Click;
            AddFolderButton.Click += AddFolderButton_Click;
            ProjectMenu.SizeChanged += ProjectMenu_SizeChanged;
        }

        private void ProjectMenu_SizeChanged(object? sender, SizeChangedEventArgs e)
        {
             DrawCanvas();
        }

        private void AddFileButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            ViewModel.AddPage();
            DrawCanvas();
        }
        private void AddFolderButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            ViewModel.AddFolder();
            DrawCanvas();
        }

        private int DrawCanvas(ObservableCollection<Node>? nodesParam = null, int counterParam = 0)
        {
            int counter;
            ObservableCollection<Node> nodes = new ObservableCollection<Node>();
            if (nodesParam == null)
            {
                nodes = ViewModel.Nodes;
                counter = 0;

                foreach (var node in nodes)
                {
                    node.IsDrew = false;
                }

                ProjectMenu.Children.Clear();
                ProjectMenu.Height = ViewModel.Nodes.Count * 40 + ViewModel.Nodes.Count * 10;
            }
            else
            {
                nodes = nodesParam;
                counter = counterParam;
            }


            foreach (var node in nodes)
            {
                if(node.IsDrew)
                {
                    continue;
                }

                Grid? button = MenuButton(node);

                if(button != null)
                {
                    Canvas.SetLeft(button, parentCount(node) * 10);
                    Canvas.SetTop(button, counter * 40 + 10);
                    node.ConnectedControl = button;
                    button.PointerPressed += Button_PointerPressed;

                    if (node.IsSelected)
                    {
                        button.Background = Avalonia.Media.Brushes.Red;
                    }

                    if (node.IsVisible)
                    {
                        // show children in tree menu
                    }


                    ProjectMenu.Children.Add(button);
                    node.IsDrew = true;

                    counter++;

                    if(node.Children.Count > 0)
                    {
                        
                        counter = DrawCanvas(node.Children, counter);
                    }
                }
            }

            return counter;
        }

        private void Button_PointerPressed(object? sender, Avalonia.Input.PointerPressedEventArgs e)
        {
            var point = e.GetCurrentPoint(sender as Control);

            if (point.Properties.IsLeftButtonPressed)
            {
                var node = ViewModel.Nodes.Where(x => x.ConnectedControl == sender as Control).FirstOrDefault();

                if(node != null)
                {
                    ViewModel.SetSelected(node);
                    DrawCanvas();
                }
            }
        }

        private Grid? MenuButton(Node node)
        {
            Grid grid = new Grid();

            try
            {
                grid.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch;
                grid.VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center;
                grid.Height = 30;
                grid.Background = Avalonia.Media.Brushes.Aqua;
                grid.Width = ProjectMenu.Bounds.Width;
                grid.ColumnDefinitions = new ColumnDefinitions("25, 25, *");

                Avalonia.Controls.Shapes.Rectangle caret = new Avalonia.Controls.Shapes.Rectangle();
                caret.Width = 25;
                caret.Height = 25;
                caret.Fill = new ImageBrush(new Avalonia.Media.Imaging.Bitmap(AssetLoader.Open(new Uri("avares://draftio/assets/caret.png"))));
                Grid.SetColumn(caret, 0);
                grid.Children.Add(caret);

                Avalonia.Controls.Image icon = new Avalonia.Controls.Image();
                if(node.Type == NodeType.File)
                {
                    icon.Source = new Avalonia.Media.Imaging.Bitmap(AssetLoader.Open(new Uri("avares://draftio/assets/file.png")));
                }
                if (node.Type == NodeType.Folder)
                {
                    icon.Source = new Avalonia.Media.Imaging.Bitmap(AssetLoader.Open(new Uri("avares://draftio/assets/folder.png")));
                }
                Grid.SetColumn(icon, 1);
                grid.Children.Add(icon);

                Label textBlock = new Label();
                textBlock.Content = "Text";
                textBlock.Height = 25;
                textBlock.Background = Avalonia.Media.Brushes.White;
                textBlock.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch;
                textBlock.VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center;

                Grid.SetColumn(textBlock, 2);
                grid.Children.Add(textBlock);
            }
            catch (Exception ex) { }


            return grid;
        }


        private int parentCount(Node node, int counter = 0)
        {
            int parentCounter = counter;

            if(node.ParentNode != null)
            {
                parentCounter++;
                return parentCount(node.ParentNode, parentCounter);
            }

            return parentCounter;

        }


        
    }
}
