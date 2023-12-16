using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using draftio.services;
using draftio.viewmodels;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Windows;

namespace draftio
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
                    services.AddTransient<MainViewModel>();
                    services.AddTransient<ProjectTreeViewModel>();
                    services.AddSingleton<TabViewModel>();
                    services.AddTransient<DrawingViewModel>();

                    services.AddSingleton<RenderManager>();
                    services.AddSingleton<ProjectManager>();
                    services.AddSingleton<ToolManager>();
                })
                .Build();

        }

        public override void Initialize()
        {

            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = new MainWindow();
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}