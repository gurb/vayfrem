using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using vayfrem.services;
using vayfrem.viewmodels;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Windows;

namespace vayfrem
{
    public partial class App : Application
    {
        public IHost AppHost
        {
            get;
        }

        public static T GetService<T>()
            where T : class
        {
            if ((App.Current as App)!.AppHost.Services.GetService(typeof(T)) is not T service)
            {
                throw new ArgumentException($"{typeof(T)} not founded in ConfigureServices");
            }

            return service;
        }

        public App()
        {
            AppHost = Host.CreateDefaultBuilder()
                .UseContentRoot(AppContext.BaseDirectory)
                .ConfigureServices((context, services) =>
                {
                    services.AddSingleton<VersionControlManager>();

                    services.AddTransient<MainViewModel>();
                    services.AddSingleton<ColorPickerManager>();

                    services.AddSingleton<FileManager>();
                    services.AddSingleton<EncodeManager>();
                    services.AddSingleton<IOManager>();

                    services.AddSingleton<LayoutViewModel>();

                    services.AddSingleton<ShortsViewModel>();
                    services.AddSingleton<TabViewModel>();
                    services.AddSingleton<DrawingViewModel>();
                    services.AddSingleton<ProjectTreeViewModel>();
                    services.AddSingleton<PropertyViewModel>();
                    services.AddSingleton<PageTreeViewModel>();
                    services.AddSingleton<ToolOptionsViewModel>();
                    services.AddSingleton<ComponentViewModel>();

                    services.AddSingleton<RenderManager>();
                    services.AddSingleton<ProjectManager>();
                    services.AddSingleton<ToolManager>();
                    services.AddSingleton<ObjectMenuManager>();

                    services.AddSingleton<UndoRedoManager>();
                    services.AddSingleton<PdfManager>();
                    services.AddSingleton<ExportManager>();

                    services.AddSingleton<MultipleUserService>();

                    services.AddSingleton<HTMLManager>();

                    services.AddSingleton<TempStorage>();

                    services.AddSingleton<RecommendLineService>();
                })
                .Build();
        }

        public override void Initialize()
        {

            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            var fileManager = App.GetService<FileManager>();

            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                fileManager.SetArgs(desktop.Args);

                desktop.MainWindow = new MainWindow();

            };

            base.OnFrameworkInitializationCompleted();
        }
    }
}