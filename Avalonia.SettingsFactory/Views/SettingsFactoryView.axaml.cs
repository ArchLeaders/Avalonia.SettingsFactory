using Avalonia.SettingsFactory.Core;
using Avalonia.SettingsFactory.ViewModels;
using Avalonia.Controls;
using System;

namespace Avalonia.SettingsFactory.Views
{
    public partial class SettingsFactoryView : SettingsFactory
    {
        [Obsolete("Only implemented to statisfy the compiler.")]
        public SettingsFactoryView() { }
        public SettingsFactoryView(SettingsFactoryViewModel binding, ISettingsValidator validator, ISettingsBase settingsBase)
        {
            InitializeComponent();

            // Very much unnecessary, but not having this bothers me.
            // Allows you to focus seemingly nothing.
            Grid focusDelegate = this.FindControl<Grid>("FocusDelegate")!;
            focusDelegate.PointerPressed += (_, _) => focusDelegate.Focus();
            Grid focusDelegate2 = this.FindControl<Grid>("FocusDelegate2")!;
            focusDelegate2.PointerPressed += (_, _) => focusDelegate.Focus();

            // Initialize settings factory
            InitializeSettingsFactory(binding, validator, settingsBase);
        }
    }
}
