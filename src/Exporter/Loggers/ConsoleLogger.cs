using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EnvSettingsManager
{
    internal class ConsoleLogger : QuietConsoleLogger, ILogger
    {
        public void LogWarning(int num, string msg)
        {
            ConsoleColor defaultConsoleColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Error.WriteLine("Warning ST{0:000}: {1}", num, msg);
            Console.ForegroundColor = defaultConsoleColor;
        }
    }
}
