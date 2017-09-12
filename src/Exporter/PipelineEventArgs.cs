using System;

namespace EnvSettingsManager
{
    public class PipelineEventArgs : EventArgs
    {
        public PipelineEventArgs(int number, string message)
        {
            Number = number;
            Message = message;
        }

        public int Number { get; set; }
        public string Message { get; set; }
    }
}
