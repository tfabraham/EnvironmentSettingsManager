using System;

namespace EnvSettingsManager
{
    /// <summary>
    /// Root class for all pipeline elements (importers, processors, exporters).
    /// Define the events that send information to the UI
    /// </summary>
    internal abstract class PipelineElement
    {
        internal event EventHandler<PipelineEventArgs> OnInfo;
        internal event EventHandler<PipelineEventArgs> OnVerbose;
        internal event EventHandler<PipelineEventArgs> OnWarning;
        internal event EventHandler<PipelineEventArgs> OnError;

        int errorCount = 0;
        int warningCount = 0;
        public int ErrorCount { get { return errorCount; } }
        public int WarningCount { get { return warningCount; } }

        protected void RaiseVerbose(string message, params object[] args)
        {
            if (OnVerbose != null)
                OnVerbose(this, new PipelineEventArgs(0, string.Format(message, args)));
        }

        protected void RaiseInfo(string message, params object[] args)
        {
            if (OnInfo != null)
                OnInfo(this, new PipelineEventArgs(0, string.Format(message, args)));
        }

        protected void RaiseWarning(string message, params object[] args)
        {
            warningCount++;
            if (OnWarning != null)
                OnWarning(this, new PipelineEventArgs(0, string.Format(message, args)));
        }

        protected void RaiseError(string message, params object[] args)
        {
            errorCount++;
            if (OnError != null)
                OnError(this, new PipelineEventArgs(0, string.Format(message, args)));
        }
    }
}