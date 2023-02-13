using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.SettingsFactory.Demo.Models;
using Avalonia.SettingsFactory.Demo.ViewModels;
using Avalonia.SettingsFactory.ViewModels;
using Avalonia.Styling;
using Avalonia.Themes.Fluent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading.Tasks;

namespace Avalonia.SettingsFactory.Demo.Views
{
    public partial class SettingsView : SettingsFactory, ISettingsValidator
    {
        public SettingsView()
        {
            AvaloniaXamlLoader.Load(this);

            // Very much unnecessary, but not having this bothers me.
            // Allows you to focus seemingly nothing.
            Grid focusDelegate = this.FindControl<Grid>("FocusDelegate")!;
            focusDelegate.PointerPressed += (_, _) => focusDelegate.Focus();
            Grid focusDelegate2 = this.FindControl<Grid>("FocusDelegate2")!;
            focusDelegate2.PointerPressed += (_, _) => focusDelegate.Focus();

            SettingsFactoryOptions options = new()
            {

                // Application implementation of a message prompt
                AlertAction = (msg) => Debug.WriteLine(msg),

                // Folder browse dialog or custom input system.
                BrowseAction = async (title) =>
                {
                    OpenFolderDialog dialog = new() { Title = title };
                    var result = await dialog.ShowAsync(App.StaticView);
                    return result;
                },

                // Custom resource loader
                FetchResource = (res) => JsonSerializer.Deserialize<Dictionary<string, string>>(File.ReadAllText(res))
            };

            // Initialize the settings layout
            InitializeSettingsFactory(new SettingsFactoryViewModel(true), this, Settings.Config, options);

            AfterSaveEvent += () => {
                // Dispose view
                (App.StaticView.DataContext as AppViewModel)!.Content = null;
            };

            AfterCancelEvent += () => {
                // Dispose view
                (App.StaticView.DataContext as AppViewModel)!.Content = null;
            };
        }

        public bool? ValidateBool(string key, bool value)
        {
            return key switch
            {
                _ => null
            };
        }

        public bool? ValidateString(string key, string value)
        {
            return key switch
            {
                "UserName" => value.All(c => (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z')),
                "FullName" => value.Contains((string)this["FirstName"]!) && value.Contains((string)this["LastName"]!),
                "Theme" => ValidateTheme(value),
                _ => null
            };
        }

        public static bool? ValidateTheme(string value)
        {
            Application.Current!.RequestedThemeVariant = value == "Dark" ? ThemeVariant.Dark : ThemeVariant.Light;
            return null;
        }

        public string? ValidateSave(Dictionary<string, bool?> validated)
        {
            if (validated["UserName"] == null)
            {
                return "Please enter a username before saving!";
            }

            // By default return a standard error message,
            // or null where all validations passed
            return validated.Where(x => x.Value == false).Any() ? "One or more settings could not be verified. Please review your settings." : null;
        }
    }
}
