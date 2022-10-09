using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Avalonia.Builders.Settings
{
    public class SettingsFactoryOptions
    {
        internal static SettingsFactoryOptions Defaults { get; } = new();

        /// <summary>
        /// Brush colour when setting validation returns true. Default <c>#FF00CC1C</c>
        /// </summary>
        public static string ValidateTrueBrush { get; set; } = "#FF00CC1C";

        /// <summary>
        /// Brush colour when setting validation returns false. Default <c>#FFFF0000</c>
        /// </summary>
        public static string ValidateFalseBrush { get; set; } = "#FFFF0000";

        /// <summary>
        /// Brush colour when setting validation returns false. Default <c>#00FFFFFF</c> (Transparent)
        /// </summary>
        public static string ValidateNullBrush { get; set; } = "#00FFFFFF";

        /// <summary>
        /// Delegate function for custom resource fetching. Default <c>() => null</c>
        /// </summary>
        public Func<string, Dictionary<string, string>?> FetchResource { get; set; } = (path) => null;

        /// <summary>
        /// Browse function for browsable string settings. Default <c>() => null</c>
        /// </summary>
        public Func<string, Task<string?>> BrowseAction { get; set; } = (name) => Task.FromResult<string?>(null);

        /// <summary>
        /// Delegate function called when alerting the interface with an error or warning. Default <c>(e) => Debug.WriteLine(e);</c>
        /// </summary>
        public Action<string> AlertAction { get; set; } = (e) => Debug.WriteLine(e);
    }
}
