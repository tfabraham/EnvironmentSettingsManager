using System.Collections.Generic;
using System.Linq;

namespace EnvSettingsManager
{
    internal class MergeProcessor : ProcessorBase
    {
        string[] environmentsToExclude;
        bool addApplicationName;
        bool raiseErrorOnConflict = true;
        bool lastWins;
        
        internal override List<SettingsFile> Process(List<SettingsFile> files, EnvSettingsManagerArguments programArguments)
        {
            MergeActionArguments args = (MergeActionArguments) programArguments;
            this.environmentsToExclude = args.excludedEnvironments;
            this.addApplicationName = args.addApplicationName;
            this.raiseErrorOnConflict = args.raiseErrorOnConflict;
            this.lastWins = args.lastWins;

            SettingsFile mergedSettings = null;

            int i = 0;
            foreach (var file in files)
            {
                i++;
                RaiseVerbose("Parsing {0}", file.Filename);

                // no appname? use file index
                if (string.IsNullOrEmpty(file.ApplicationName))
                    file.ApplicationName = string.Format("APP{0:00}", i);

                RaiseVerbose("Merging file data");
                mergedSettings = Merge(mergedSettings, file);
            }//for

            if (args.sort)
            {
                mergedSettings.Settings = mergedSettings.Settings
                    .OrderBy(s => s.Key)
                    .ToDictionary(s => s.Key, s => s.Value);
            }

            var ret = new List<SettingsFile>();
            ret.Add(mergedSettings);
            return ret;
        }

        internal SettingsFile Merge(SettingsFile file1, SettingsFile file2)
        {
            if (file1 == null)
                return file2;

            var environmentKeys = file2.Environments.Keys.Union(file1.Environments.Keys);
            var settingKeys = file2.Settings.Keys.Union(file1.Settings.Keys);

            environmentKeys = environmentKeys.Where(key => !environmentsToExclude.Contains(key));

            SettingsFile merge = new SettingsFile();
            foreach (var settingKey in settingKeys)
            {
                Setting winnerSetting;
                string winnerKey;

                if (file1.Settings.ContainsKey(settingKey))
                {
                    winnerSetting = file1.Settings[settingKey];
                    winnerSetting.Name = MakeKey(file1.ApplicationName, winnerSetting.Name);
                    winnerKey = MakeKey(file1.ApplicationName, settingKey);
                }
                else
                {
                    winnerSetting = file2.Settings[settingKey];
                    winnerSetting.Name = MakeKey(file2.ApplicationName, winnerSetting.Name);
                    winnerKey = MakeKey(file2.ApplicationName, settingKey);
                }
                merge.Settings.Add(winnerKey, winnerSetting);
            }

            foreach (var environmentKey in environmentKeys)
            {
                EnvironmentSettings environment = MergeEnvironment(file1, file2, settingKeys, environmentKey);

                merge.Environments.Add(environmentKey, environment);
            }//for

            merge.ApplicationName = string.Empty;

            return merge;
        }

        private EnvironmentSettings MergeEnvironment(SettingsFile file1, SettingsFile file2, IEnumerable<string> settingKeys, string environmentKey)
        {
            EnvironmentSettings environment = new EnvironmentSettings();

            EnvironmentSettings env1 = null, env2 = null;
            if (file1.Environments.ContainsKey(environmentKey))
                env1 = file1.Environments[environmentKey];
            if (file2.Environments.ContainsKey(environmentKey))
                env2 = file2.Environments[environmentKey];

            environment.EnvironmentName = env1 != null ? env1.EnvironmentName : env2.EnvironmentName;
            //TODO conflicts warnings
            environment.Filename = env1 != null ? env1.Filename : env2.Filename;
            environment.GenerateFile = env1 != null ? env1.GenerateFile : env2.GenerateFile;
            foreach (var settingKey in settingKeys)
            {
                Setting v1 = null, v2 = null;
                if (env1 != null && env1.Settings.ContainsKey(settingKey))
                    v1 = env1.Settings[settingKey];
                if (env2 != null && env2.Settings.ContainsKey(settingKey))
                    v2 = env2.Settings[settingKey];
                if (v1 != null && v2 != null)
                {
                    string message;
                    if (v1.Value != v2.Value)
                    {
                        if (raiseErrorOnConflict)
                        {
                            message = string.Format(
                                "101: Setting '{0}' has conflicting values '{1}' and '{2}' from '{3}' and '{4}', respectively.",
                                v1.Name, v1.Value, v2.Value, v1.Originator, v2.Originator);
                            RaiseError(message);
                            //throw new ApplicationException(message);
                        }
                        else
                        {
                            if (lastWins)
                            {
                                message = string.Format(
                                    "003: Setting '{0}' has conflicting values '{1}' and '{2}' from '{3}' and '{4}', respectively: using '{2}'.",
                                    v1.Name, v1.Value, v2.Value, v1.Originator, v2.Originator);
                                RaiseWarning(message);
                                v1 = null;
                            }
                            else
                            {
                                message = string.Format(
                                    "002: Setting '{0}' has conflicting values '{1}' and '{2}' from '{3}' and '{4}', respectively: using '{1}'.",
                                    v1.Name, v1.Value, v2.Value, v1.Originator, v2.Originator);
                                RaiseWarning(message);
                                v2 = null;
                            }
                        }
                    }
                    else
                    {
                        message = string.Format(
                            "001: Setting '{0}' is already defined in '{1}'.",
                            v1.Name, v1.Originator);
                        RaiseWarning(message);
                    }
                }
                if (v1 != null)
                    environment.Settings.Add(MakeKey(file1.ApplicationName, settingKey), v1);
                else if (v2 != null)
                    environment.Settings.Add(MakeKey(file2.ApplicationName, settingKey), v2);
            }//for

            return environment;
        }

        private string MakeKey(string applicationName, string settingKey)
        {
            if (!addApplicationName || applicationName == string.Empty)
                return settingKey;
            return string.Format("{0}.{1}", applicationName, settingKey);
        }


    }
}
