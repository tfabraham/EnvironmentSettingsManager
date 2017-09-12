using System;

namespace EnvSettingsManager
{
    internal class QueryProcessor
    {
        public static SettingsFile Select(SettingsFile settingsFile, string environmentKey)
        {
            var environment = settingsFile.Environments[environmentKey];
            SettingsFile projection = new SettingsFile() {
                Description = environment.EnvironmentName,
                Filename = environment.Filename,
                ApplicationName = settingsFile.ApplicationName
            };
            projection.Environments.Add(environmentKey,environment);
            projection.Settings = settingsFile.Settings;

            return projection;
        }

        public static SettingsFile Filter(SettingsFile settingsFile, string[] settings)
        {
            throw new NotImplementedException();
        }
    }
}
