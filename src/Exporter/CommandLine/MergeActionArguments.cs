using CommandLine;

namespace EnvSettingsManager
{
    internal class MergeActionArguments : EnvSettingsManagerArguments
    {
        [Argument(ArgumentType.Multiple, ShortName = "x", LongName = "exclude", HelpText = "Environment(s) to exclude from merge result.")]
        public string[] excludedEnvironments;
        [Argument(ArgumentType.AtMostOnce, ShortName = "s", HelpText = "Alphabetical sort of settings.")]
        public bool sort;
        [Argument(ArgumentType.AtMostOnce, ShortName = "a", LongName = "applicationPrefix", HelpText = "Prefix application name to settings.")]
        public bool addApplicationName;
        [Argument(ArgumentType.AtMostOnce, LongName = "raiseErrorOnConflict", DefaultValue = true, HelpText = "Conflicts raise errors.")]
        public bool raiseErrorOnConflict;
        [Argument(ArgumentType.AtMostOnce, LongName = "lastWins", DefaultValue = false, HelpText = "If raiseErrorOnConflict is false, last value found wins.")]
        public bool lastWins;
    }
}
