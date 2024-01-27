using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using System;
using vayfrem.services;

namespace vayfrem
{
    public partial class MainWindow : Window
    {
        private readonly VersionControlManager versionControlManager;

        public MainWindow()
        {

            versionControlManager = App.GetService<VersionControlManager>();

            InitializeComponent();

            Title = "Vayfrem " + versionControlManager.Version;

            this.Closed += MainWindow_Closed;
        }

        private void MainWindow_Closed(object? sender, System.EventArgs e)
        {
            GC.Collect();
        }
    }
}