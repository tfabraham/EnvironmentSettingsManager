using System;
using System.IO;

namespace EnvSettingsManager
{
    internal abstract class ImporterBase : PipelineElement
    {
        internal abstract SettingsFile ImportSettings(string inputFileName);

        protected static SettingsFile CreateSettingsFile(string A1)
        {
            SettingsFile settings = new SettingsFile();
            settings.Description = A1;
            // determine empirically appname
            string appName = A1 ?? string.Empty;
            appName = appName.Replace("Environment Settings for ", "");
            appName = appName.Replace("Environment Settings", "");
            appName = appName.Trim();
            settings.ApplicationName = appName;
            return settings;
        }

        protected static string RegisterSetting(SettingsFile settingsFile, Setting setting)
        {
            // define setting key
            string key = setting.Name.ToLowerInvariant();
            if (settingsFile.Settings.ContainsKey(key))
            {
                throw new ArgumentException("Duplicate setting " + setting.Name + ".");
            }
            settingsFile.Settings.Add(key, setting);
            return key;
        }


        /// <summary>
        /// Convert the string value of the "Generate File?" setting to a boolean.
        /// </summary>
        /// <param name="value">String value</param>
        /// <returns>Value converted to boolean</returns>
        protected virtual bool ParseGenerateFileValue(string value)
        {
            // No value defaults to true.
            if (string.IsNullOrEmpty(value))
            {
                return true;
            }

            // Either Yes or True (case-insensitive) are accepted as Boolean true
            if (string.Compare("yes", value, StringComparison.InvariantCultureIgnoreCase) == 0
                || string.Compare("true", value, StringComparison.InvariantCultureIgnoreCase) == 0)
            {
                return true;
            }

            return false;
        }
    }
}
