using System;

namespace EnvSettingsManager
{
	class IOBinder
	{
        ILogger logger;

        internal IOBinder(ILogger logger)
        {
            this.logger = logger;
        }

		internal void Bind(PipelineElement element)
		{
			element.OnError += new EventHandler<PipelineEventArgs>(OnPipelineError);
			element.OnWarning += new EventHandler<PipelineEventArgs>(OnPipelineWarning);
			element.OnInfo += new EventHandler<PipelineEventArgs>(OnPipelineInfo);
			element.OnVerbose += new EventHandler<PipelineEventArgs>(OnPipelineVerbose);
		}

		void OnPipelineError(object sender, PipelineEventArgs e)
		{
            logger.LogError(e.Number, e.Message);
		}

		void OnPipelineWarning(object sender, PipelineEventArgs e)
		{
            logger.LogWarning(e.Number, e.Message);
		}

		void OnPipelineInfo(object sender, PipelineEventArgs e)
		{
            logger.LogInfo(e.Message);
		}

		void OnPipelineVerbose(object sender, PipelineEventArgs e)
		{
            logger.LogVerbose(e.Message);
		}
	}
}
