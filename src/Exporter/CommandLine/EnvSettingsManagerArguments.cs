using CommandLine;

namespace EnvSettingsManager
{
    /// <summary>
    /// Actions or mode for the program
    /// </summary>
    /// <remarks>
    /// Every enum value must have a corresponding class derived from <see cref="EnvSettingsManagerArguments"/>.
    /// Also <see cref="PipelineFactory.MakeImporter"/> could change.
    /// </remarks>
    internal enum ProgramAction
    {
        Export,
        Merge
    }

    internal class EnvSettingsManagerArguments
    {
        // not parsed by Parser
        internal ProgramAction action;

        [Argument(ArgumentType.AtMostOnce, ShortName = "n", HelpText = "Hide this message.")]
        public bool nologo;
        [Argument(ArgumentType.Required | ArgumentType.Multiple, ShortName = "i", LongName = "input", HelpText = "Input file(s) to merge.")]
        public string[] inputFiles;
        [Argument(ArgumentType.Required, ShortName = "o", LongName = "output", HelpText = "Output file.")]
        public string outputFile;
        [Argument(ArgumentType.AtMostOnce, DefaultValue = false, ShortName = "v", HelpText = "Verbose output.")]
        public bool verbose;
        [Argument(ArgumentType.AtMostOnce, DefaultValue = true, ShortName = "w", HelpText = "Enable merge warnings.")]
        public bool warnings;
    }
}
