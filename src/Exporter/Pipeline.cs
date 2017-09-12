using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EnvSettingsManager
{
    internal class Pipeline
    {
        List<PipelineElement> elements = new List<PipelineElement>();
        IOBinder binder;

        public Pipeline(ILogger logger)
        {
            binder = new IOBinder(logger);
        }

        public ImporterBase AddImporter(string inputFileName)
        {
            ImporterBase importer = PipelineFactory.MakeImporter(inputFileName);
            binder.Bind(importer);
            elements.Add(importer);
            return importer;
        }

        public ExporterBase AddExporter(EnvSettingsManagerArguments programArguments)
        {
            ExporterBase exporter = PipelineFactory.MakeExporter(programArguments);
            binder.Bind(exporter);
            elements.Add(exporter);
            return exporter;
        }

        public ProcessorBase AddProcessor(ProgramAction programMode)
        {
            ProcessorBase processor = PipelineFactory.MakeProcessor(programMode);
            binder.Bind(processor);
            elements.Add(processor);
            return processor;
        }

        public int ErrorCount
        {
            get
            {
                return elements.Sum(e => e.ErrorCount);
            }
        }
    }
}
