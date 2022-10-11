using Avalonia.Controls;
using Avalonia.SettingsFactory.Demo.Views;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avalonia.SettingsFactory.Demo.ViewModels
{
    public class AppViewModel : ReactiveObject
    {
        private UserControl? content = null;
        public UserControl? Content {
            get => content;
            set => this.RaiseAndSetIfChanged(ref content, value);
        }

        public void ShowSettings()
        {
            Content = new SettingsView();
        }
    }
}
