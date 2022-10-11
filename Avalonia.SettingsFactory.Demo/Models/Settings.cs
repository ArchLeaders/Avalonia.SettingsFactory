using Avalonia.SettingsFactory.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Avalonia.SettingsFactory.Demo.Models
{
    public class Settings : ISettingsBase
    {
        public static Settings Config { get; set; } = new();
        public bool RequiresInput { get; set; }

        [Setting("User Name", "Must only contain a-Z characters.", ShowBrowseButton = false)]
        public string? UserName { get; set; }

        [Setting("First Name", "Your first name.", ShowBrowseButton = false)]
        public string FirstName { get; set; } = "";

        [Setting("Last Name", "Your last name.", ShowBrowseButton = false)]
        public string LastName { get; set; } = "";

        [Setting("Full Name", "Your full name. (Must contain your First and Last name)", ShowBrowseButton = false)]
        public string? FullName { get; set; }

        [Setting(UiType.Dropdown, "Dark", "Light", Category = "Appearance")]
        public string Theme { get; set; } = "Dark";

        [Setting("Random Path", "Some random folder on your PC", Category = "Random Category", Folder = "Random Folder", ShowBrowseButton = true)]
        public string RandomPath { get; set; } = "";

        public ISettingsBase Save()
        {
            Debug.WriteLine(JsonSerializer.Serialize(this));
            return this;
        }
    }
}
