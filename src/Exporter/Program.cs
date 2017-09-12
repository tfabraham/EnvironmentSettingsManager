// (c) Copyright 2007-10 Thomas F. Abraham.
// This source is subject to the Microsoft Public License (Ms-PL).
// See http://www.opensource.org/licenses/ms-pl.html
// All other rights reserved.

using System;
using System.Data;
using System.IO;

namespace EnvironmentSettingsExporter
{
    /// <summary>
    /// This application reads the deployment environment settings from an EnvironmentSettings.xls/.xml Excel file
    /// and exports them to one or more XML files.
    /// </summary>
    class Program
    {
        // Export file format
        private enum FormatType
        {
            // XmlPreprocess format
            XmlPreprocess,
            // .NET appSettings XML format
            AppSettings,
            // WiX CustomTable format
            WixCustomTable
        };

        /// <summary>
        /// Main entry point.
        /// </summary>
        /// <param name="args">Command-line parameters</param>
        /// <returns>
        /// 0 for successful completion
        /// -1 for invalid argument list
        /// -2 for exception during processing
        /// </returns>
        static int Main(string[] args)
        {
            ConsoleColor defaultConsoleColor = Console.ForegroundColor;

            Version assemblyVersion = System.Reflection.Assembly.GetEntryAssembly().GetName().Version;

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(
                "Environment Settings Spreadsheet to XML Exporter "
                + assemblyVersion.Major + "." + assemblyVersion.Minor + "." + assemblyVersion.Build);
            Console.WriteLine("[http://EnvSettingsManager.codeplex.com]");
            Console.WriteLine("Copyright (C) 2007-11 Thomas F. Abraham.  All Rights Reserved.");
            Console.WriteLine();
            Console.ForegroundColor = defaultConsoleColor;

            if (args.Length < 2 || args.Length > 3)
            {
                PrintCommandLineHelp();
                return -1;
            }

            string inputFile = args[0];
            string outputPath = args[1];

            FormatType activeFormat = FormatType.XmlPreprocess;

            // Handle any additional command-line parameters
            if (args.Length > 2)
            {
                // Still assuming a fixed location for the Format parameter
                string formatParam = args[2]; // Any more and we'll need a command line parser

                string[] formatParamSplit = formatParam.Split(':');
                if (string.Compare(formatParamSplit[0], "/F", StringComparison.InvariantCultureIgnoreCase) == 0)
                {
                    try
                    {
                        activeFormat = (FormatType)Enum.Parse(typeof(FormatType), formatParamSplit[1], true);
                    }
                    catch (ArgumentException)
                    {
                        PrintCommandLineHelp();
                        return -1;
                    }
                }
            }

            Console.WriteLine("Importing from " + Path.GetFileName(inputFile) + "...");
            Console.WriteLine();

            try
            {
                // Open the source file and read the settings into a DataTable.
                DataTable settingsTable = SettingsFileReader.ReadSettingsFromExcelFile(inputFile);

                if (!Directory.Exists(outputPath))
                {
                    Directory.CreateDirectory(outputPath);
                }

                DataTableToXmlExporter exporter = null;

                if (activeFormat == FormatType.AppSettings)
                {
                    Console.WriteLine("Output format is .NET AppSettings (multi-file).");
                    Console.WriteLine();

                    exporter = new DataTableToAppSettingsXmlExporter();
                    exporter.ExportSettings(settingsTable, outputPath, Path.GetFileName(inputFile), false, null);
                }
                else if (activeFormat == FormatType.WixCustomTable)
                {
                    Console.WriteLine("Output format is WiX CustomTable (single file).");
                    Console.WriteLine();

                    exporter = new DataTableToWixCustomTableXmlExporter();
                    exporter.ExportSettings(settingsTable, outputPath, Path.GetFileName(inputFile), true, "EnvironmentSettings.wxi");
                }
                else
                {
                    Console.WriteLine("Output format is XmlPreprocess (multi-file).");
                    Console.WriteLine();

                    exporter = new DataTableToXmlPreprocessXmlExporter();
                    exporter.ExportSettings(settingsTable, outputPath, Path.GetFileName(inputFile), false, null);
                }
            }
            catch (Exception ex)
            {
                string errorMessage = "ERROR: " + ex.Message;

                // Capture all inner exception messages too.
                while (ex.InnerException != null)
                {
                    ex = ex.InnerException;
                    errorMessage += "; " + ex.Message;
                }

                // Write the error message to the console in red.
                ConsoleColor originalColor = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(errorMessage);
                Console.ForegroundColor = originalColor;

                return -2;
            }

            Console.WriteLine();
            Console.WriteLine("Finished.");
            
            return 0;
        }

        private static void PrintCommandLineHelp()
        {
            Console.WriteLine("Usage: EnvironmentSettingsExporter.exe <ExcelFile.xls/x or ExcelFile.xml> <OutputPath> [/F:<XmlPreprocess/AppSettings/WixCustomTable>]");
        }
    }
}
