using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Avalonia.SettingsFactory
{
    /// <summary>
    /// Base interface for custom validation logic.
    /// </summary>
    public interface ISettingsValidator
    {
        /// <summary>
        /// Custom validation logic function for string setting properties.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool? ValidateString(string key, string value);

        /// <summary>
        /// Custom validation logic function for bool setting properties.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool? ValidateBool(string key, bool value);

        /// <summary>
        /// Implement custom checks for your settings before saving. (Return null if all checks passed.)
        /// </summary>
        /// <param name="validated"></param>
        /// <returns></returns>
        public string? ValidateSave(Dictionary<string, bool?> validated);
    }
}
