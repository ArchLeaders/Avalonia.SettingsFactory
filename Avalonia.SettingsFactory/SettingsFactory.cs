#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

using Avalonia.SettingsFactory.Core;
using Avalonia.SettingsFactory.Extensions;
using Avalonia.SettingsFactory.ViewModels;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Markup.Xaml.MarkupExtensions;
using Avalonia.Layout;
using Avalonia.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Avalonia.SettingsFactory
{
    public class SettingsFactory : UserControl
    {
        public SettingsFactoryOptions Options { get; set; }
        public event Action? AfterSaveEvent;
        public event Action? AfterCancelEvent;

        internal SettingsFactoryViewModel Binding => (SettingsFactoryViewModel)DataContext!;
        internal string? ReferenceKey;
        internal ISettingsBase SettingsBase;
        internal ISettingsValidator Validator;
        internal ToggleButton? DefaultButton = null;
        internal ToggleButton? LastButton = null;
        internal StackPanel Root;
        internal readonly Dictionary<string, List<SettingsElementFactory>> References = new();
        internal readonly Dictionary<string, SettingsElementFactory> Settings = new();
        internal readonly Dictionary<string, StackPanel> Folders = new();
        internal readonly Dictionary<string, StackPanel> Panels = new();
        internal readonly List<string> Categories = new();

        public SettingsFactory() { }

        public void InitializeSettingsFactory(SettingsFactoryViewModel binding, ISettingsValidator validator, ISettingsBase settingsBase, SettingsFactoryOptions? options = null)
        {
            DataContext = binding;
            Validator = validator;
            SettingsBase = settingsBase;
            Options = options ?? SettingsFactoryOptions.Defaults;

            Root = this.FindControl<StackPanel>("Root")!;
            this.FindControl<Button>("Save")!.Click += OnSave;
            this.FindControl<Button>("Cancel")!.Click += (s, e) => AfterCancelEvent?.Invoke();

            var props = SettingsBase.GetType().GetProperties().Where(x => x.GetCustomAttributes<SettingAttribute>(false).Any());

            foreach (var prop in props) {
                CreateElement(prop);
            }
        }

        public object? this[string name] {
            get {
                if (ReferenceKey != null) {
                    if (!References.ContainsKey(name)) {
                        References.Add(name, new());
                    }

                    if (!References[name].Contains(Settings[ReferenceKey])) {
                        References[name].Add(Settings[ReferenceKey]);
                    }
                }

                return Settings.ContainsKey(name) ? Settings[name].GetElementValue() : null;
            }
        }

        public virtual void OnSave(object? sender, EventArgs e)
        {
            var check = Validator.ValidateSave(Settings.ToDictionary(x => x.Key, x => x.Value.ValidateElement()));
            if (check != null) {
                Options.AlertAction(check);
                return;
            }
            else {
                foreach ((var name, var value) in Settings) {
                    SettingsBase.GetType().GetProperty(name)?.SetValue(SettingsBase, value.GetElementValue());
                }
            }

            SettingsBase.RequiresInput = false;
            SettingsBase.Save();
            AfterSaveEvent?.Invoke();
        }

        //
        // Validation Helpers

        internal void ValidateReferences(string? referenceKey)
        {
            if (ReferenceKey != null && References.ContainsKey(ReferenceKey)) {
                foreach (var element in References[ReferenceKey]) {
                    element.ValidateElement();
                }
            }
        }

        internal bool? Validate(string key, object? value, bool validateContext = false)
        {
            ReferenceKey = key;
            if (validateContext) {
                ValidateReferences(key);
            }

            if (value != null) {
                if (value is string str) {
                    return Validator.ValidateString(key, str);
                }
                else if (value is bool boolean) {
                    return Validator.ValidateBool(key, boolean);
                }
                else {
                    throw new ArgumentException($"Objects of type '{value.GetType().Name ?? "null"}' are unsupported by the settings validator.", nameof(value));
                }
            }

            return null;
        }

        //
        // Factory Builders

        internal void CreateElement(PropertyInfo prop)
        {
            SettingsElementFactory element = new(this, prop, SettingsBase);
            SettingAttribute setting = element.Setting;
            string key = $"{setting.Folder.SafeName()}.{setting.Category.SafeName()}";

            // Get element panel
            var panel = Panels.ContainsKey(key) ? Panels[key] : CreatePanel(setting.Folder, setting.Category);

            // Get header folder
            var folder = Folders.ContainsKey(setting.Folder.SafeName()) ? Folders[setting.Folder.SafeName()] : CreateFolderPanel(setting.Folder);

            // Create category navigation button
            if (!Categories.Contains(setting.Category.SafeName())) {
                CreateCategoryNav(key, folder, setting.Category);
            }

            // Add element factory
            Settings.Add(prop.Name, element);
            element.CreateElement(panel);
        }

        internal StackPanel CreateFolderPanel(string folder)
        {
            var panel = new StackPanel();

            TextBlock titleBlock = new() {
                Text = folder,
                FontWeight = FontWeight.Bold,
                Margin = new(8, 30, 0, 12)
            };
            titleBlock[!TextBlock.ForegroundProperty] = new DynamicResourceExtension("SystemControlForegroundChromeGrayBrush");
            panel.Children.Add(titleBlock);

            Border splitter = new() {
                Height = 1,
                Margin = new(2, 0, 2, 5)
            };
            splitter[!TextBlock.BackgroundProperty] = new DynamicResourceExtension("SystemChromeHighColor");
            panel.Children.Add(splitter);

            Root.Children.Add(panel);
            Folders.Add(folder.SafeName(), panel);

            return panel;
        }

        internal void CreateCategoryNav(string key, StackPanel folder, string category)
        {
            ToggleButton categoryButton = new() {
                Name = category.SafeName(),
                Content = category,
                Margin = new(0, 2, 0, 0),
                HorizontalAlignment = HorizontalAlignment.Stretch,
                Background = Brushes.Transparent
            };

            categoryButton.Click += (s, e) => {
                if (categoryButton.IsChecked == false) {
                    categoryButton.IsChecked = !categoryButton.IsChecked;
                }
                else {
                    if (LastButton != null) {
                        LastButton.IsChecked = false;
                    }
                    LastButton = categoryButton;
                    Binding.ActiveElement = Panels[key];
                }
            };

            LastButton ??= categoryButton;
            if (DefaultButton == null) {
                DefaultButton = categoryButton;
                DefaultButton.IsChecked = true;
                Binding.ActiveElement = Panels[key];
            }

            folder.Children.Add(categoryButton);
            Categories.Add(category.SafeName());
        }

        internal StackPanel CreatePanel(string folder, string category)
        {
            string key = $"{folder.SafeName()}.{category.SafeName()}";

            StackPanel panel = new() {
                Margin = new Thickness(30, 30, 40, 30)
            };

            TextBlock titleBlock = new() {
                Margin = new(0, 0, 0, 25),
                FontSize = 20,
                FontWeight = FontWeight.Medium,
                Text = category
            };
            panel.Children.Add(titleBlock);

            Panels.Add(key, panel);

            return panel;
        }
    }
}
