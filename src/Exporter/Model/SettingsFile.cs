using System.Collections.Generic;

namespace EnvSettingsManager
{
    /// internal representation of a whole settings file
    internal partial class SettingsFile
    {
        public static string DefaultValueEnvironmentName = "DEFAULT VALUES";

        public string Filename { get; set; }
        public string ApplicationName { get; set; }
        public string Description { get; set; }
        public Dictionary<string, EnvironmentSettings> Environments = new Dictionary<string, EnvironmentSettings>();
        // Value is not used in this collection!
        public Dictionary<string, Setting> Settings = new Dictionary<string, Setting>();
    }
}
