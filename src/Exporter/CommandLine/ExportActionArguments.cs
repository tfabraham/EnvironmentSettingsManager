using CommandLine;

namespace EnvSettingsManager
{
    // Export file format
    internal enum FormatType
    {
        // XmlPreprocess format
        XmlPreprocess,
        // .NET appSettings XML format
        AppSettings,
        // WiX CustomTable format
        WixCustomTable
    }

    internal class ExportActionArguments : EnvSettingsManagerArguments
    {
        [Argument(ArgumentType.AtMostOnce, DefaultValue = FormatType.XmlPreprocess, ShortName = "f", LongName = "format", HelpText = "Format of output file.")]
        public FormatType format;
    }
}
