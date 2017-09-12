using System;
using System.Collections.Generic;

namespace EnvSettingsManager
{
    internal abstract class ProcessorBase : PipelineElement
    {
        internal abstract List<SettingsFile> Process(List<SettingsFile> files, EnvSettingsManagerArguments programArguments);
    }
}
