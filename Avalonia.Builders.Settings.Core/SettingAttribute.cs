namespace Avalonia.Builders.Settings.Core
{
    public enum UiType
    {
        Default,
        TextBox,
        Dropdown,
        Toggle,
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class SettingAttribute : Attribute
    {
        public string Category { get; set; } = "General";
        public string Folder { get; set; } = "Settings";
        public string? Name { get; set; } = null;
        public string Description { get; set; } = "";
        public UiType UiType { get; set; } = UiType.Default;
        public string[]? DropdownElements { get; set; } = null;
        public bool ShowBrowseButton { get; set; } = true;

        public SettingAttribute() { }

        public SettingAttribute(string name) => Name = name;

        public SettingAttribute(UiType uiType, params object[] dropdownElements)
        {
            DropdownElements = (from obj in dropdownElements select obj.ToString()).ToArray();
            UiType = uiType;
        }

        public SettingAttribute(string name, string description)
        {
            Name = name;
            Description = description;
        }
    }
}
