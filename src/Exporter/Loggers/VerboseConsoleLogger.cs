using System;

namespace EnvSettingsManager
{
    internal class VerboseConsoleLogger : ConsoleLogger, ILogger
    {
        //void ILogger.LogVerbose(string msg)
        public new void LogVerbose(string msg)
        {
            ConsoleColor defaultConsoleColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("VERBOSE: ");
            Console.WriteLine(msg);
            Console.ForegroundColor = defaultConsoleColor;
        }
    }
}
