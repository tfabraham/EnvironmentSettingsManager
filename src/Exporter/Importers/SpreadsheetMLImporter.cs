using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.XPath;

namespace EnvSettingsManager
{
    /// <summary>
    /// Reads a SpreadsheetML XML file (Office 2003 format or older).
    /// </summary>
    internal class SpreadsheetMLImporter : ImporterBase
    {
        const string ss = SpreadsheetMLConstants.ss;

        internal override SettingsFile ImportSettings(string inputFileName)
        {
            RaiseInfo("Loading {0}", inputFileName);
            return ReadFile(inputFileName);
        }

        #region READING

        internal SettingsFile ReadFile(string filename)
        {
            // Read the SpreadsheetML XML file into a new XPathDocument.
            XPathDocument doc = new XPathDocument(filename);

            XPathNavigator nav = doc.CreateNavigator();

            // Check the validity of the XML
            if (!IsValidSpreadsheetMl(nav))
            {
                //BUG no file name
                throw new ArgumentException("The input file is not a valid SpreadsheetML file or it is an unsupported version.");
            }

            // Create a namespace manager and register the SpreadsheetML namespace and prefix
            XmlNamespaceManager nm = new XmlNamespaceManager(nav.NameTable);
            nm.AddNamespace("ss", "urn:schemas-microsoft-com:office:spreadsheet");

            // Locate the Settings worksheet
            XPathNavigator worksheetNav = nav.SelectSingleNode("//ss:Worksheet[@ss:Name='Settings']/ss:Table", nm);
            if (worksheetNav == null)
            {
                throw new ArgumentException("The input file does not contain a worksheet entitled Settings.");
            }

            List<EnvironmentSettings> environments = ReadHeaders(worksheetNav, nm);

            SettingsFile settings = CreateSettingsFile(worksheetNav, nm);
            settings.Filename = filename;

            ReadTable(settings, environments, worksheetNav, nm);

            foreach (var environment in environments)
            {
                settings.Environments.Add(environment.EnvironmentName.ToUpperInvariant(), environment);
            }

            return settings;
        }

        private static SettingsFile CreateSettingsFile(XPathNavigator worksheetNav, XmlNamespaceManager nm)
        {
            SettingsFile settings = new SettingsFile();
            // Select the first cell (A1)
            XPathNavigator firstRow =
                worksheetNav.SelectSingleNode("//ss:Row[1]/ss:Cell[1]/ss:Data", nm);
            return CreateSettingsFile(firstRow.Value);
        }


        /// <summary>
        /// Read the header contained in the first 4 rows of the Execl sheet.
        /// </summary>
        /// <param name="worksheetNav"></param>
        /// <param name="nm"></param>
        /// <returns></returns>
        private List<EnvironmentSettings> ReadHeaders(XPathNavigator worksheetNav, XmlNamespaceManager nm)
        {
            //use a list to preserve column order
            var environments = new List<EnvironmentSettings>();

            int index;

            // Select the row that contains the environment names
            XPathNavigator environmentNamesRow =
                worksheetNav.SelectSingleNode("//ss:Row[ss:Cell[ss:Data = 'Environment Name:']]", nm);
            if (environmentNamesRow == null)
            {
                throw new ArgumentException(
                    "The input file does not contain a row with a value in the first column equal to 'Environment Name:'.");
            }

            // Select all of the cells in the row
            XPathNodeIterator cellsIterator = environmentNamesRow.Select("ss:Cell/ss:Data", nm);
            // skip column 0
            cellsIterator.MoveNext();
            while (cellsIterator.MoveNext())
            {
                var environment = new EnvironmentSettings();
                // pick cell value
                environment.EnvironmentName = cellsIterator.Current.Value;
                environments.Add(environment);
            }//for
            //fixup
            environments[0].EnvironmentName = SettingsFile.DefaultValueEnvironmentName;


            // Select the row that contains the generate
            XPathNavigator generateFileRow =
                worksheetNav.SelectSingleNode("//ss:Row[ss:Cell[ss:Data = 'Generate File?']]", nm);
            if (generateFileRow == null)
            {
                throw new ArgumentException(
                    "The input file does not contain a row with a value in the first column equal to 'Generate File?'.");
            }
            // Select all of the cells in the row
            cellsIterator = generateFileRow.Select("ss:Cell", nm);
            // skip column 0
            cellsIterator.MoveNext();
            index = 0;
            while (cellsIterator.MoveNext() && index < environments.Count)
            {
                // pick cell value
                string generateFile = cellsIterator.Current.Value;
                environments[index].GenerateFile = ParseGenerateFileValue(generateFile);
                index++;
            }//for


            // Select the row that contains the file name
            XPathNavigator fileNameRow =
                worksheetNav.SelectSingleNode("//ss:Row[ss:Cell[ss:Data = 'Settings File Name:']]", nm);
            if (fileNameRow == null)
            {
                throw new ArgumentException(
                    "The input file does not contain a row with a value in the first column equal to 'Settings File Name:'.");
            }
            // Select all of the cells in the row
            cellsIterator = fileNameRow.Select("ss:Cell", nm);
            // skip column 0
            cellsIterator.MoveNext();
            index = 0;
            while (cellsIterator.MoveNext() && index < environments.Count)
            {
                // pick cell value
                environments[index].Filename = cellsIterator.Current.Value;
                index++;
            }//for

            return environments;
        }


        /// <summary>
        /// Read the Settings data rows.
        /// </summary>
        /// <param name="environments"></param>
        /// <param name="worksheetNav"></param>
        /// <param name="nm"></param>
        /// <returns></returns>
        private static void ReadTable(SettingsFile settingsFile, List<EnvironmentSettings> environments, XPathNavigator worksheetNav, XmlNamespaceManager nm)
        {
            // Select all of the rows in the worksheet
            XPathNodeIterator rowsIterator = worksheetNav.Select("//ss:Row", nm);

            // skip header rows until we find Settings
            var cell0 = rowsIterator.Current.SelectSingleNode("ss:Cell[1]/ss:Data", nm);
            while (cell0 == null || cell0.Value != "Settings:")
            {
                if (rowsIterator.MoveNext())
                {
                    cell0 = rowsIterator.Current.SelectSingleNode("ss:Cell[1]/ss:Data", nm);
                }
                else
                {
                    throw new ArgumentException("The input file does not contain a row with a value in the first column equal to 'Settings:'.");
                }
            }

            // Loop through the rows
            while (rowsIterator.MoveNext())
            {
                // Select all of the cells in the row
                XPathNodeIterator cellsIterator = rowsIterator.Current.Select("ss:Cell", nm);

                // this is the null Value object used to list all possibile settings names
                Setting setting = new Setting() { Originator = settingsFile.ApplicationName };

                // Look for setting name
                if (!cellsIterator.MoveNext())
                {
                    throw new ArgumentException("The input file has an empty row.");
                }
                XPathNavigator dataNav = cellsIterator.Current.SelectSingleNode("ss:Data", nm);
                if (dataNav == null)
                {
                    // no setting cell -> end of table
                    break;
                }

                setting.Name = dataNav.Value;
                // Select the comment value in the cell, if present
                XPathNavigator commentNav = cellsIterator.Current.SelectSingleNode("ss:Comment/ss:Data", nm);
                if (commentNav != null)
                {
                    setting.Comment = commentNav.Value;
                }

                string key = RegisterSetting(settingsFile, setting);

                // count from first env column
                int columnIndex = 0;
                // Loop through the remaining cells
                while (cellsIterator.MoveNext())
                {
                    // Select the data value in the cell, if present
                    string indexAttr = cellsIterator.Current.GetAttribute("Index", ss);
                    if (!string.IsNullOrEmpty(indexAttr))
                    {
                        // index = 1 === column A
                        // columnIndex = 0 === column B (i.e. Default Values environment)
                        columnIndex = Convert.ToInt32(indexAttr) - 2;
                    }

                    // Select the data value in the cell, if present
                    dataNav = cellsIterator.Current.SelectSingleNode("ss:Data", nm);
                    if (dataNav != null)
                    {
                        // environment has only "valued" cells
                        var cell = new Setting() { Name = setting.Name, Value = dataNav.Value, Originator = settingsFile.ApplicationName };

                        // Select the comment value in the cell, if present
                        commentNav = cellsIterator.Current.SelectSingleNode("ss:Comment/ss:Data", nm);
                        if (commentNav != null)
                        {
                            cell.Comment = commentNav.Value;
                        }

                        environments[columnIndex].Settings.Add(key, cell);
                    }

                    columnIndex++;
                }
            }
        }

        /// <summary>
        /// Check for a valid Excel mso-application processing instruction.
        /// </summary>
        /// <param name="nav"></param>
        /// <returns></returns>
        private static bool IsValidSpreadsheetMl(XPathNavigator nav)
        {
            bool isValid = false;

            XPathNodeIterator piIterator = nav.SelectChildren(XPathNodeType.ProcessingInstruction);

            while (piIterator.MoveNext())
            {
                if (string.Compare(piIterator.Current.LocalName, "mso-application", true) == 0)
                {
                    if (string.Compare(piIterator.Current.Value, "progid=\"Excel.Sheet\"", true) == 0)
                    {
                        isValid = true;
                    }
                }
            }

            return isValid;
        }

        #endregion

    }
}
