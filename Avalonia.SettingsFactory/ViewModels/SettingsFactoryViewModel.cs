using Avalonia.Controls;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avalonia.SettingsFactory.ViewModels
{
    public class SettingsFactoryViewModel : ReactiveObject
    {
        private StackPanel? activeElement;
        public StackPanel? ActiveElement {
            get => activeElement;
            set => this.RaiseAndSetIfChanged(ref activeElement, value);
        }

        private bool canCancel;
        public bool CanCancel {
            get => canCancel;
            set => this.RaiseAndSetIfChanged(ref canCancel, value);
        }

        public SettingsFactoryViewModel(bool canCancel) => this.canCancel = canCancel;
    }
}
