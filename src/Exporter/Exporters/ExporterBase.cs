using System;
using System.Collections.Generic;

namespace EnvSettingsManager
{
    internal abstract class ExporterBase : PipelineElement
    {
        internal abstract void ExportSettings(List<SettingsFile> settingsFiles, string outputPath);
    }
}
