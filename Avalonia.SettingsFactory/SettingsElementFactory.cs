#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

using Avalonia.SettingsFactory.Core;
using Avalonia.SettingsFactory.Extensions;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Avalonia.SettingsFactory
{
    internal class SettingsElementFactory
    {
        internal Border ValidationElement;
        internal Control Element;
        internal SettingAttribute Setting;
        internal SettingsFactory Factory;
        internal object? Value;
        internal string Name;

        public SettingsElementFactory(SettingsFactory factory, PropertyInfo prop, object? settings)
        {
            Factory = factory;
            Name = prop.Name;
            Value = prop.GetValue(settings);
            Setting = prop.GetCustomAttribute<SettingAttribute>()!;

            Setting.Name ??= Name;
        }
        
        public object? GetElementValue()
        {
            if (Element is TextBox textBox) {
                return textBox.Text;
            }
            else if (Element is ComboBox comboBox) {
                return (comboBox.SelectedItem as ComboBoxItem)?.Tag;
            }
            else if (Element is ToggleSwitch toggle) {
                return toggle.IsChecked;
            }
            else {
                throw new NotImplementedException($"Parsing controls of type '{Element.GetType().Name}' are not supported.");
            }
        }

        public bool? ValidateElement()
        {
            bool? result = Factory.Validate(Name, GetElementValue());
            ValidationElement.Background = result.ToBrush();
            return result;
        }

        public void CreateElement(StackPanel panel)
        {
            var element = Setting.UiType switch {
                UiType.Default =>
                    CreateDefaultElement(),
                UiType.TextBox =>
                    CreateTextElement(Value as string),
                UiType.Dropdown =>
                    CreateDropdownElement(Value as string),
                UiType.Toggle =>
                    CreateToggleElement((bool)Value!),
                _ => throw new NotImplementedException()
            };

            panel.Children.Add(element);
        }

        private Border CreateDefaultElement()
        {
            if (Value is bool boolean) {
                return CreateToggleElement(boolean);
            }
            else {
                return CreateTextElement(Value as string);
            }
        }

        private Border CreateTextElement(string? value)
        {
            Border root = CreateBaseElement();
            Grid grid = (Grid)root.Child!;

            TextBox element = new() {
                VerticalAlignment = VerticalAlignment.Top
            };
            element.GetObservable(TextBox.TextProperty).Subscribe(text => ValidationElement.Background = Factory.Validate(Name, text, true).ToBrush());
            element.Text = value;
            Grid.SetColumn(element, 3);

            if (Setting.ShowBrowseButton) {
                Button browse = new() {
                    Content = "...",
                    Height = 32,
                    Width = 32,
                    HorizontalContentAlignment = HorizontalAlignment.Center,
                    VerticalContentAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Right,
                    VerticalAlignment = VerticalAlignment.Top
                };
                browse.Click += async (s, e) => element.Text = await Factory.Options.BrowseAction(Setting.Name!) ?? element.Text;
                Grid.SetColumn(browse, 4);

                grid.Children.Add(browse);
            }
            else {
                Grid.SetColumnSpan(element, 2);
            }

            grid.Children.Add(element);

            Element = element;
            return root;
        }

        private Border CreateDropdownElement(string? value)
        {
            List<ComboBoxItem> items = new();
            string[]? elements = Setting.DropdownElements;
            int index = 0;

            if (elements != null) {
                for (int i = 0; i < elements.Length; i++) {

                    if (elements[i].Contains(':') && Factory.Options.FetchResource(elements[i].Split(':')[1]) is Dictionary<string, string> resource) {
                        int pos = 0;
                        foreach ((var _tag, var _value) in resource) {
                            items.Add(new() { Content = _value, Tag = _tag });
                            if (_tag == value) {
                                index = pos;
                            }
                            pos++;
                        }
                    }
                    else {
                        items.Add(new() { Content = elements[i], Tag = elements[i] });
                        if (elements[i] == value) {
                            index = i;
                        }
                    }
                }
            }

            Border root = CreateBaseElement();
            Grid grid = (Grid)root.Child!;

            ComboBox element = new() {
                Items = items,
                VerticalAlignment = VerticalAlignment.Top,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                Padding = new Thickness(10, 1, 0, 0),
            };
            element.SelectionChanged += (s, e) => ValidationElement.Background = Factory.Validate(Name, (element.SelectedItem as ComboBoxItem)?.Tag!.ToString(), true).ToBrush();
            element.SelectedIndex = index;
            Grid.SetColumn(element, 3);
            Grid.SetColumnSpan(element, 2);

            grid.Children.Add(element);

            Element = element;
            return root;
        }

        private Border CreateToggleElement(bool value)
        {
            Border root = CreateBaseElement("6,*,10,80,37");
            Grid grid = (Grid)root.Child!;

            ToggleSwitch element = new() {
                IsChecked = value,
                VerticalAlignment = VerticalAlignment.Top,
                HorizontalAlignment = HorizontalAlignment.Right,
                OnContent = "",
                OffContent = ""
            };
            element.Click += (s, e) => ValidationElement.Background = Factory.Validate(Name, element.IsChecked, true).ToBrush();
            Grid.SetColumn(element, 3);
            Grid.SetColumnSpan(element, 2);

            grid.Children.Add(element);

            Element = element;
            return root;
        }

        private Border CreateBaseElement(string columns = "6,*,10,1.2*,37")
        {
            Border root = new() {
                CornerRadius = new(5),
                Padding = new(10),
                Margin = new(0, 0, 0, 15)
            };
            root.Classes.Add("ElementBase");

            Grid child = new() {
                ColumnDefinitions = new(columns)
            };

            StackPanel texts = new();
            Grid.SetColumn(texts, 1);

            ValidationElement = new() {
                CornerRadius = new(1),
                Width = 2,
                HorizontalAlignment = HorizontalAlignment.Left,
                Margin = new(-2, 0, 0, 0),
                Background = Brushes.Transparent
            };

            TextBlock nameBlock = new() {
                Text = Setting.Name,
                MaxWidth = 880,
                TextWrapping = TextWrapping.Wrap,
                Margin = new(0, 0, 0, 5)
            };

            TextBlock descBlock = new() {
                Text = Setting.Description,
                MaxWidth = 880,
                TextWrapping = TextWrapping.Wrap,
                VerticalAlignment = VerticalAlignment.Bottom,
                Opacity = 0.5,
                FontWeight = FontWeight.Light,
                FontSize = 11
            };

            texts.Children.Add(nameBlock);
            texts.Children.Add(descBlock);
            child.Children.Add(ValidationElement);
            child.Children.Add(texts);
            root.Child = child;

            return root;
        }
    }
}
