using System.Collections.Generic;

namespace EnvSettingsManager
{
    // represents a column
    internal class EnvironmentSettings
    {
        public string EnvironmentName { get; set; }
        public bool GenerateFile { get; set; }//i.e. Active
        public string Filename { get; set; }
        public Dictionary<string, Setting> Settings = new Dictionary<string, Setting>();
    }
}
