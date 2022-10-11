using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Avalonia.SettingsFactory.Demo.Views
{
    public partial class AppView : Window
    {
        public AppView()
        {
            AvaloniaXamlLoader.Load(this);
            this.AttachDevTools();
        }
    }
}
