namespace Avalonia.Builders.Settings.Core
{
    public interface ISettingsBase
    {
        public bool RequiresInput { get; set; }
        public ISettingsBase Save();
    }
}
