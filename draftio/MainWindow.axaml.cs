using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using System;

namespace draftio
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            this.Closed += MainWindow_Closed;
        }

        private void MainWindow_Closed(object? sender, System.EventArgs e)
        {
            GC.Collect();
        }
    }
}