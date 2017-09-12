using System;
using System.Collections.Generic;

namespace EnvSettingsManager
{
    internal class Program
    {
        internal static int Main(string[] args)
        {
            ILogger logger = new ConsoleLogger();
            try
            {
                CommandLineParser cmdLineParser = new CommandLineParser(logger);
                EnvSettingsManagerArguments programArguments = cmdLineParser.ParseCommandLine(args);
                if (programArguments == null)
                {
                    //invalid args
                    return 42;
                }
                if (programArguments.verbose)
                {
                    // switch logger
                    logger = new VerboseConsoleLogger();
                }
                if (!programArguments.warnings)
                {
                    // switch logger
                    logger = new QuietConsoleLogger();
                }

                Pipeline pipeline = new Pipeline(logger);

                // step 1: load input file(s) in memory
                List<SettingsFile> files = new List<SettingsFile>();
                foreach (var inputFile in programArguments.inputFiles)
                {
                    // determine Importer to use
                    ImporterBase importer = pipeline.AddImporter(inputFile);
                    // import data
                    SettingsFile settingsFile = importer.ImportSettings(inputFile);
                    // store
                    files.Add(settingsFile);
                }

                // step 2: process file(s) in-memory representation
                ProcessorBase processor = pipeline.AddProcessor(programArguments.action);
                List<SettingsFile> processedSettingsFiles = processor.Process(files, programArguments);

                if (pipeline.ErrorCount == 0)
                {
                    // step 3: write output file(s)
                    ExporterBase exporter = pipeline.AddExporter(programArguments);
                    //BUG output is written also in case of processing errors
                    exporter.ExportSettings(processedSettingsFiles, programArguments.outputFile);
                    return 0;
                }
                else
                {
                    logger.LogInfo("Found error(s): no output has been generated.");
                    return 1;
                }//if errors

            }
            catch (Exception e)
            {
                logger.LogError(999, "Fatal error: " + e.Message);
                // unexpected
                return 99;
            }
        }
    }
}
