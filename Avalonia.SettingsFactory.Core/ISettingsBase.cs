namespace Avalonia.SettingsFactory.Core
{
    public interface ISettingsBase
    {
        public bool RequiresInput { get; set; }
        public ISettingsBase Save();
    }
}
