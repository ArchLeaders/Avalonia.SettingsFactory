using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Avalonia.SettingsFactory.Extensions
{
    internal static class NameExt
    {
        internal static Dictionary<string, string> InvalidChars { get; set; } = new() {
            { " ", "_" }
        };

        internal static string SafeName(this string name)
        {
            foreach (var invalidChar in InvalidChars) {
                name = name.Replace(invalidChar.Key, invalidChar.Value);
            }

            return name;
        }
    }
}
