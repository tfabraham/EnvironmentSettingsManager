// Copyright 2007 Thomas F. Abraham. All Rights Reserved.
// See LICENSE.txt for licensing information.

using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Xml;
using System.Xml.XPath;

namespace EnvironmentSettingsExporter
{
    /// <summary>
    /// Reads a SpreadsheetML XML file (Office 2003 format or older).
    /// </summary>
    internal static class SpreadsheetMlSettingsFileReader
    {
        /// <summary>
        /// Read the settings from Office 2003 SpreadsheetML XML file into a DataTable.
        /// </summary>
        /// <param name="inputFile"></param>
        /// <returns></returns>
        internal static DataTable ReadSettingsFromSpreadsheetMl(string inputFile)
        {
            // Read the SpreadsheetML XML file into a new XPathDocument.
            XPathDocument doc = new XPathDocument(inputFile);

            // Create a new, empty DataTable to hold the data. The resulting structure of the DataTable will
            // be identical to that produced with a binary XLS file.
            DataTable dt = null;

            XPathNavigator nav = doc.CreateNavigator();

            // Check the validity of the XML
            if (!IsValidSpreadsheetMl(nav))
            {
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

            dt = InitializeDataTable(worksheetNav, nm);

            // Select all of the rows in the worksheet
            XPathNodeIterator rowsIterator = worksheetNav.Select("//ss:Row", nm);

            // Loop through the rows
            while (rowsIterator.MoveNext())
            {
                // Select all of the cells in the row
                XPathNodeIterator cellsIterator = rowsIterator.Current.Select("ss:Cell", nm);

                int columnIndex = 0;

                // Create a new DataRow to hold the incoming values
                DataRow newRow = dt.NewRow();

                // Loop through the cells
                while (cellsIterator.MoveNext())
                {
                    // Select the comment value in the cell, if present
                    XPathNavigator commentNav = cellsIterator.Current.SelectSingleNode("ss:Comment/ss:Data", nm);

                    if (commentNav != null)
                    {
                        string commentValue = commentNav.Value;

                        if (!string.IsNullOrEmpty(commentValue))
                        {
                            newRow["Comment"] = commentNav.Value.Replace("\n", string.Empty);
                        }
                    }

                    // Check for Index attribute on Cell
                    if (cellsIterator.Current.HasAttributes)
                    {
                        XPathNavigator indexAttribute = cellsIterator.Current.SelectSingleNode("@ss:Index", nm);

                        if (indexAttribute != null)
                        {
                            // SpreadsheetML stores Index as 1 based, not zero based.
                            columnIndex = int.Parse(indexAttribute.Value) - 1;
                        }
                    }

                    // Select the data value in the cell, if present
                    XPathNavigator dataNav = cellsIterator.Current.SelectSingleNode("ss:Data", nm);

                    if (dataNav != null)
                    {
                        newRow[columnIndex] = dataNav.Value;
                    }

                    columnIndex++;
                }

                // Add the newly populated DataRow to the DataTable
                dt.Rows.Add(newRow);
            }

            return dt;
        }

        /// <summary>
        /// Initialize a DataTable that can hold data for each environment defined in the spreadsheet plus comments.
        /// </summary>
        /// <param name="worksheetNav"></param>
        /// <param name="nm"></param>
        /// <returns>Initialized DataTable</returns>
        private static DataTable InitializeDataTable(XPathNavigator worksheetNav, XmlNamespaceManager nm)
        {
            DataTable dt = new DataTable("Settings");

            // Select the row that contains the environment names
            XPathNavigator environmentNamesRow =
                worksheetNav.SelectSingleNode("//ss:Row[ss:Cell[ss:Data = 'Environment Name:']]", nm);

            if (environmentNamesRow == null)
            {
                throw new ArgumentException(
                    "The input file does not contain a row with a value in the first column equal to 'Environment Name:'.");
            }

            // Select all of the cells in the row
            XPathNodeIterator cellsIterator = environmentNamesRow.Select("ss:Cell", nm);

            // Initialize the DataTable by creating new columns to hold the incoming data
            // Add a column for each cell, plus an extra column to hold cell comments
            for (int index = 0; index < cellsIterator.Count; index++)
            {
                dt.Columns.Add(index.ToString(), typeof(string));
            }

            dt.Columns.Add("Comment", typeof(string));

            return dt;
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
    }
}
