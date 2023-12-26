using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Platform;
using draftio.models;
using draftio.models.enums;
using draftio.viewmodels;
using System;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;

namespace draftio.views.sections
{
    public partial class ProjectTreeView : UserControl
    {
        ProjectTreeViewModel ViewModel { get; set; }

        TimeSpan lastClickTime = new TimeSpan();
        Node? lastClickNode;
        
        public ProjectTreeView()
        {
            ViewModel = App.GetService<ProjectTreeViewModel>();
            ViewModel.drawProjectView += RefreshDraw;
            DataContext = ViewModel;

            InitializeComponent();

            AddFileButton.Click += AddFileButton_Click;
            AddFolderButton.Click += AddFolderButton_Click;
            ProjectMenu.SizeChanged += ProjectMenu_SizeChanged;

            setStyle();
        }

        private void setStyle ()
        {
            projectMenuHeader.Background = new SolidColorBrush(Avalonia.Media.Color.FromArgb(255, 204, 208, 209));
            projectMenuFooter.Background = new SolidColorBrush(Avalonia.Media.Color.FromArgb(255, 204, 208, 209));
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

        private void RefreshDraw()
        {
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
                ProjectMenu.Height = ViewModel.Nodes.Count * 26;
            }
            else
            {
                nodes = nodesParam;
                counter = counterParam;
            }


            foreach (var node in nodes)
            {
                if (node.IsVisible == false)
                {
                    iterateVisibleNode(node);
                }

                if (node.IsDrew)
                {
                    continue;
                }
            

                Grid? button = MenuButton(node);

                if(button != null)
                {
                    Canvas.SetLeft(button, parentCount(node) * 20);
                    Canvas.SetTop(button, counter * 26);
                    node.ConnectedMenuControl = button;
                    button.PointerPressed += Button_PointerPressed;

                    ProjectMenu.Children.Add(button);

                    if (node.IsSelected)
                    {
                        Avalonia.Controls.Shapes.Rectangle hover = new Avalonia.Controls.Shapes.Rectangle();
                        hover.Fill = new SolidColorBrush(Avalonia.Media.Color.FromArgb(100, 100, 100, 100));
                        hover.Stroke = Avalonia.Media.Brushes.Gray;
                        hover.StrokeThickness = 2;
                        hover.Width = ProjectMenu.Bounds.Width;
                        hover.Height = 26;
                        hover.IsEnabled = false;
                        Canvas.SetLeft(hover, 0);
                        Canvas.SetTop(hover, counter * 26);
                        ProjectMenu.Children.Add(hover);
                    }

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

                var node = ViewModel.Nodes.Where(x => x.ConnectedMenuControl == sender as Control).FirstOrDefault();

                TimeSpan timeSinceLastClick = DateTime.Now.TimeOfDay - lastClickTime;
                if (node != null && node.Type == NodeType.File && timeSinceLastClick.TotalMilliseconds < 300 && lastClickNode == node)
                {
                    ViewModel.SetSelected(node);
                }
                lastClickTime = DateTime.Now.TimeOfDay;


                if (node != null)
                {
                    if(node.Type == NodeType.Folder)
                    {
                        ViewModel.SetSelected(node);
                    }
                    else
                    {
                        ViewModel.SetSelectedHover(node);
                    }

                    lastClickNode = node;
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
                grid.Height = 25;
                grid.Background = Avalonia.Media.Brushes.White;
                grid.Width = ProjectMenu.Bounds.Width;
                grid.ColumnDefinitions = new ColumnDefinitions("25, 25, *");


                if(node.Type == NodeType.Folder)
                {
                    Avalonia.Controls.Shapes.Rectangle caret = new Avalonia.Controls.Shapes.Rectangle();
                    caret.Width = 16;
                    caret.Height = 16;
                    caret.PointerPressed += Caret_PointerPressed;
                    node.CaretControl = caret;
                    if (node.IsVisible)
                    {
                        caret.Fill = new ImageBrush(new Avalonia.Media.Imaging.Bitmap(AssetLoader.Open(new Uri("avares://draftio/assets/careton.png"))));
                    }
                    else
                    {
                        caret.Fill = new ImageBrush(new Avalonia.Media.Imaging.Bitmap(AssetLoader.Open(new Uri("avares://draftio/assets/caret.png"))));
                    }

                    Grid.SetColumn(caret, 0);
                    grid.Children.Add(caret);
                }

                Avalonia.Controls.Image icon = new Avalonia.Controls.Image();
                icon.Width = 20;
                icon.Height = 20;
                if (node.Type == NodeType.File)
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
                textBlock.Content = node.Name;
                textBlock.Height = 20;
                textBlock.FontSize = 12;
                textBlock.Background = Avalonia.Media.Brushes.Transparent;
                textBlock.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch;
                textBlock.VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center;

                Grid.SetColumn(textBlock, 2);
                grid.Children.Add(textBlock);
            }
            catch (Exception ex) { }


            return grid;
        }

        private void Caret_PointerPressed(object? sender, Avalonia.Input.PointerPressedEventArgs e)
        {
            var point = e.GetCurrentPoint(sender as Control);

            if (point.Properties.IsLeftButtonPressed)
            {
                var node = ViewModel.Nodes.Where(x => x.CaretControl == sender as Control).FirstOrDefault();

                if (node != null)
                {
                    node.IsVisible = !node.IsVisible;
                    
                    DrawCanvas();
                }
            }
        }


        private void iterateVisibleNode (Node node)
        {
            foreach (var child in node.Children)
            {
                child.IsDrew = true;

                iterateVisibleNode(child);
            }
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
