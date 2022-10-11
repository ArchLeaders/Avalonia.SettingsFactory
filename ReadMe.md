# Avalonia SettingsFactory

[![NuGet](https://img.shields.io/nuget/v/AvaloniaSettingsFactory.svg)](https://www.nuget.org/packages/AvaloniaSettingsFactory) [![NuGet](https://img.shields.io/nuget/dt/AvaloniaSettingsFactory.svg)](https://www.nuget.org/packages/AvaloniaSettingsFactory)

**Avalonia SettingsFactory** is a dynamic UI library that lets you seamlessly implement a settings editor in your [Avalonia](https://github.com/AvaloniaUI/Avalonia) application using an existing settings object.

https://user-images.githubusercontent.com/80713508/195053696-49c2be25-ef9a-49ae-ab6e-263a6c53f59a.mp4

## Usage

Avalonia SettingsFactory works reading a decorated [settings object](https://github.com/ArchLeaders/Avalonia.SettingsFactory/blob/master/Avalonia.SettingsFactory.Demo/Models/Settings.cs) and sending the results into a [custom view](https://github.com/ArchLeaders/Avalonia.SettingsFactory/blob/master/Avalonia.SettingsFactory.Demo/Views/SettingsView.axaml.cs) that inherits the `SettingsFactory` UserControl included in this library.

The SettingsFactory component can be initialized with the `InitializeSettingsFactory` function and an optional `SettingsFactoryOptions` instance to configure the front-end actions used by the internal library.

```cs
public partial class SettingsView : SettingsFactory, ISettingsValidator

// ...

InitializeComponent();

// ...

SettingsFactoryOptions options = new() {

    // Application implementation of a message prompt
    AlertAction = (msg) => Debug.WriteLine(msg),

    // Folder browse dialog or custom input system.
    BrowseAction = async (title) => {
        OpenFolderDialog dialog = new() { Title = title };
        var result = await dialog.ShowAsync(App.StaticView);
        return result;
    },

    // Custom resource loader (must always return a Dictionary<string, string>)
    FetchResource = (res) => JsonSerializer.Deserialize<Dictionary<string, string>>(File.ReadAllText(res))

    // Implement custom logic to occur after saving or cancelling
    AfterSaveEvent += () => {
        // Dispose view after saving
        (App.StaticView.DataContext as AppViewModel).Content = null;
    };

    AfterCancelEvent += () => {
        // Dispose view after cancelling
        (App.StaticView.DataContext as AppViewModel).Content = null;
    };
};

// Initialize the settings layout
InitializeSettingsFactory(
    new SettingsFactoryViewModel(canCancel: true), // Build-in or custom ViewModel inheriting SettingsFactoryViewModel
    this, // implemented ISettingsValidator
    Settings.Config, // Your decorated settings object
    options
);
```

<br>

Each setting (property) can also be validated with custom logic using the ISettingsValidator interface and SettingsFactory indexing.

```cs
// Custom validation for specified settings
// in your base settings object
public bool? ValidateString(string key, string value)
{
    return key switch
    {
        // Execute a custom check.
        "UserName" => value.All(c => (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z')),

        // You can also reference other properties using the SettingsFactory indexing.
        "FullName" => value.Contains((string)this["FirstName"]!) && value.Contains((string)this["LastName"]!),
        
        // Run a custom action (e.g. change the application theme)
        "Theme" => ValidateTheme(value),

        // Or return null (default, blank color)
        _ => null
    };
}

// Validate bool properties
public bool? ValidateBool(string key, bool value) { /* ... */ }

// Check all the properties before saving
// and warn the user about specific settings
public string? ValidateSave(Dictionary<string, bool?> validated)
{
    // Singled-out property logic
    if (validated["UserName"] == null) {
        return "Please enter a username before saving!";
    }

    // Optionally do one final check on all properties and return a generic error.
    // If all property values validated true, return null to continue saving.
    return validated.Where(x => x.Value == false).Any() ? "One or more settings could not be verified. Please review your settings." : null;
}
```

<br>

_\* Note: Setting properties without a [Setting](https://github.com/ArchLeaders/Avalonia.SettingsFactory/blob/master/Avalonia.SettingsFactory.Core/SettingAttribute.cs) attribute will not be read!_

_\* Check out the [Demo](https://github.com/ArchLeaders/tree/master/Avalonia.SettingsFactory.Demo/) application for a complete implementation._

## Install

Install with NuGet or build from [source]().

#### NuGet
```powershell
Install-Package AvaloniaSettingsFactory
Install-Package AvaloniaSettingsFactory.Core
```


#### Build from Source
```batch
git clone https://github.com/ArchLeaders/Avalonia.SettingsFactory.git
dotnet build Avalonia.SettingsFactory
```

---

**© 2022 Arch Leaders**
