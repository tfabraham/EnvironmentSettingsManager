using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EnvSettingsManager
{
    internal class QuietConsoleLogger : ILogger
    {
        public void LogError(int num, string msg)
        {
            ConsoleColor defaultConsoleColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Error.WriteLine("Error ST{0:000}: {1}", num, msg);
            Console.ForegroundColor = defaultConsoleColor;
        }

        public void LogInfo(string msg)
        {
            ConsoleColor defaultConsoleColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine(msg);
            Console.ForegroundColor = defaultConsoleColor;
        }

        public void LogVerbose(string msg)
        {
            //no-op
        }

        public void LogWarning(int num, string msg)
        {
            //no-op
        }
    }
}
