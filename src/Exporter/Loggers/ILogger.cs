using System;

namespace EnvSettingsManager
{
    interface ILogger
    {
        void LogError(int num, string msg);
        void LogInfo(string msg);
        void LogVerbose(string msg);
        void LogWarning(int num, string msg);
    }
}
