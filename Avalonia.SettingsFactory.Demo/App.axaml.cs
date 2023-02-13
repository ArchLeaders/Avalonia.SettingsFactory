using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.SettingsFactory.Demo.ViewModels;
using Avalonia.SettingsFactory.Demo.Views;
using Avalonia.Themes.Fluent;
using System;

namespace Avalonia.SettingsFactory.Demo
{
    public partial class App : Application
    {
        public static AppView StaticView { get; set; } = null!;

        public override void Initialize() => AvaloniaXamlLoader.Load(this);

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop) {
                StaticView = new() {
                    DataContext = new AppViewModel()
                };

                desktop.MainWindow = StaticView;
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}
