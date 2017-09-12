using System;
using System.Collections.Generic;

namespace EnvSettingsManager
{
    internal class NullProcessor : ProcessorBase
    {
        internal override List<SettingsFile> Process(List<SettingsFile> files, EnvSettingsManagerArguments programArguments)
        {
            ExportActionArguments args = (ExportActionArguments)programArguments;
            if (files.Count != 1)
            {
                throw new ArgumentException("Export mode allows only one input file.");
            }

            //no-op
            return files;
        }
    }
}
