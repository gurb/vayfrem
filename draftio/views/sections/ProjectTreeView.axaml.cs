using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using draftio.viewmodels;

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
                Rectangle rect = new Rectangle();
                rect.Height = 35;
                rect.Fill = Brushes.Aqua;
                rect.Width = ProjectMenu.Bounds.Width;
                rect.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch;
                Canvas.SetLeft(rect, 0);
                Canvas.SetTop(rect, counter * 40 + 10);


                ProjectMenu.Children.Add(rect);

                counter++;
            }

        }
    }
}
