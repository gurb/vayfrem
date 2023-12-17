using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using draftio.viewmodels;
using System;
using System.Diagnostics.Metrics;

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

            AddButton.Click += AddButton_Click;
            ProjectMenu.SizeChanged += ProjectMenu_SizeChanged;
        }

        private void ProjectMenu_SizeChanged(object? sender, SizeChangedEventArgs e)
        {
             DrawCanvas();
        }

        private void AddButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            ViewModel.AddPage();
            DrawCanvas();
        }

        private void DrawCanvas()
        {
            ProjectMenu.Children.Clear();


            int counter = 0;

            ProjectMenu.Height = ViewModel.Nodes.Count * 40 + ViewModel.Nodes.Count * 10;


            foreach (var node in ViewModel.Nodes)
            {
                Grid button = MenuButton();
                Canvas.SetLeft(button, 0);
                Canvas.SetTop(button, counter * 40 + 10);

                ProjectMenu.Children.Add(button);

                counter++;
            }

        }


        private Grid MenuButton()
        {
            Grid grid = new Grid();

            try
            {

                grid.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch;
                grid.VerticalAlignment = Avalonia.Layout.VerticalAlignment.Stretch;
                grid.Height = 30;
                grid.Background = Brushes.Aqua;
                grid.Width = ProjectMenu.Bounds.Width;
                grid.ColumnDefinitions = new ColumnDefinitions("50, 50, *");


                Rectangle caret = new Rectangle();
                caret.Width = 25;
                caret.Height = 25;
                caret.Fill = new ImageBrush(new Bitmap(AssetLoader.Open(new Uri("avares://draftio/assets/zoomin.png"))));
                Grid.SetColumn(caret, 0);
                grid.Children.Add(caret);

                Image icon = new Image();
                icon.Source = new Bitmap(AssetLoader.Open(new Uri("avares://draftio/assets/zoomout.png")));
                Grid.SetColumn(icon, 1);
                grid.Children.Add(icon);

                TextBlock textBlock = new TextBlock();
                textBlock.Text = "Text";
                textBlock.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch;
                textBlock.VerticalAlignment = Avalonia.Layout.VerticalAlignment.Stretch;
                Grid.SetColumn(textBlock, 2);
                grid.Children.Add(textBlock);
            }
            catch (Exception ex) { }


            return grid;
        }


        
    }
}
