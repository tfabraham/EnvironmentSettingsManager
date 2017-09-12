using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace EnvSettingsManager
{
    internal static class PipelineFactory
    {
        internal static ImporterBase MakeImporter(string inputFileName)
        {
            ImporterBase importer = null;
            if (!File.Exists(inputFileName))
            {
                throw new FileNotFoundException("The specified input file " + inputFileName + " does not exist.", inputFileName);
            }

            string fileExtension = Path.GetExtension(inputFileName);

            if (string.Compare(fileExtension, ".xls", true) == 0 || string.Compare(fileExtension, ".xlsx", true) == 0)
            {
                importer = new ExcelBinaryImporter();
            }
            else if (string.Compare(fileExtension, ".xml", true) == 0)
            {
                importer = new SpreadsheetMLImporter();
            }

            return importer;
        }

        internal static ExporterBase MakeExporter(EnvSettingsManagerArguments programArguments)
        {
            ExporterBase exporter = null;
            switch (programArguments.action)
            {
                case ProgramAction.Export:
                    ExportActionArguments args = (ExportActionArguments)programArguments;
                    switch (args.format)
                    {
                        case FormatType.XmlPreprocess:
                            exporter = new XmlPreprocessExporter(args);
                            break;
                        case FormatType.AppSettings:
                            exporter = new AppSettingsExporter(args);
                            break;
                        case FormatType.WixCustomTable:
                            exporter = new WixCustomTableExporter(args);
                            break;
                        default:
                            throw new ArgumentException("Unsupported format");
                    }//inner switch
                    break;
                case ProgramAction.Merge:
                    exporter = new SpreadsheetMLExporter();
                    break;
                default:
                    throw new ArgumentException("Unsupported mode");
            }//outer switch
            return exporter;
        }

        internal static ProcessorBase MakeProcessor(ProgramAction programMode)
        {
            ProcessorBase processor = null;
            switch (programMode)
            {
                case ProgramAction.Export:
                    processor = new NullProcessor();
                    break;
                case ProgramAction.Merge:
                    processor = new MergeProcessor();
                    break;
                default:
                    throw new ArgumentException("Unsupported Mode: " + programMode.ToString());
            }
            return processor;
        }

    }
}
