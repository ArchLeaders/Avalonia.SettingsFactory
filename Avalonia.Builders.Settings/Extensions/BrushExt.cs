using Avalonia.Media;
using System;

namespace Avalonia.Builders.Settings.Extensions
{
    internal static class BrushExt
    {
        internal static Brush ToBrush(this string color) => new SolidColorBrush(Convert.ToUInt32(color.Replace("#", "").PadLeft(8, 'F'), 16));
        internal static Brush? ToBrush(this bool? value)
        {
            return (value switch {
                true => SettingsFactoryOptions.ValidateTrueBrush,
                false => SettingsFactoryOptions.ValidateFalseBrush,
                _ => SettingsFactoryOptions.ValidateNullBrush,
            }).ToBrush();
        }
    }
}
