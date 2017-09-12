using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;

namespace EnvSettingsManager
{
    /// <summary>
    /// Reads a binary Excel workbook file.
    /// </summary>
    class ExcelBinaryImporter : ImporterBase
    {
        private const int SettingNameColumnIndex = 0;
        private const int DefaultValueColumnIndex = 1;

        private const int EnvironmentNameRowIndex = 1;
        private const int GenerateFileRowIndex = 2;
        private const int FilenameRowIndex = 3;
        private const int FirstValueRowIndex = 6;

        internal override SettingsFile ImportSettings(string inputFileName)
        {
            RaiseInfo("Loading {0}", inputFileName);
            DataTable dt = ReadSettingsFromXls(inputFileName);
            RaiseVerbose("Converting datatable to internal format");
            SettingsFile sf = ConvertDataTableToSettingsFile(dt);
            RaiseVerbose("Conversion completed");
            sf.Filename = inputFileName;
            return sf;
        }

        #region DATATABLE CONVERTER

        private SettingsFile ConvertDataTableToSettingsFile(DataTable settingsTable)
        {
            // Select the first cell (A1)
            // that's the file comment
            string A1 = (string)settingsTable.Rows[0][0];

            SettingsFile settingsFile = CreateSettingsFile(A1);

            // parse Header
            List<EnvironmentSettings> environments = new List<EnvironmentSettings>();
            //fixup
            environments.Add(new EnvironmentSettings() { EnvironmentName = SettingsFile.DefaultValueEnvironmentName });
            for (int columnIndex = DefaultValueColumnIndex + 1;
                 columnIndex < settingsTable.Columns.Count && !settingsTable.Rows[FilenameRowIndex].IsNull(columnIndex);
                 columnIndex++)
            {
                var environment = new EnvironmentSettings();
                // pick cell value
                var generateCell = settingsTable.Rows[GenerateFileRowIndex][columnIndex];
                bool generateFile = ParseGenerateFileValue((string)generateCell);
                string environmentName = (string)settingsTable.Rows[EnvironmentNameRowIndex][columnIndex];
                string outputFileName = (string)settingsTable.Rows[FilenameRowIndex][columnIndex];

                environment.EnvironmentName = environmentName;
                environment.GenerateFile = generateFile;
                environment.Filename = outputFileName;
                environments.Add(environment);
            }

            // Loop through the rows that contain settings and get the name
            for (int rowIndex = FirstValueRowIndex; rowIndex < settingsTable.Rows.Count; rowIndex++)
            {
                // this is the null Value object used to list all possibile settings names
                Setting setting = new Setting() { Originator = settingsFile.ApplicationName };
                // Determine the setting name, or skip the row if there is no setting name value.
                if (settingsTable.Rows[rowIndex].IsNull(SettingNameColumnIndex))
                {
                    continue;
                }
                string settingName = (string)settingsTable.Rows[rowIndex][SettingNameColumnIndex];
                if (settingName.Trim().Length == 0)
                {
                    continue;
                }
                setting.Name = settingName;
                // Determine the comment value, or skip to the data if there is no comment value.
                if (!settingsTable.Rows[rowIndex].IsNull("Comment"))
                {
                    setting.Comment = settingsTable.Rows[rowIndex]["Comment"].ToString();
                }

                string key = RegisterSetting(settingsFile, setting);

                for (int columnIndex = DefaultValueColumnIndex;
                     columnIndex < settingsTable.Columns.Count && !settingsTable.Rows[FilenameRowIndex].IsNull(columnIndex);
                     columnIndex++)
                {
                    // environment has only "valued" cells
                    if (!settingsTable.Rows[rowIndex].IsNull(columnIndex))
                    {
                        // Determine the setting value.
                        string settingValue = ((string)settingsTable.Rows[rowIndex][columnIndex]).Trim();
                        var cell = new Setting() { Name = setting.Name, Value = settingValue, Originator = settingsFile.ApplicationName };
                        // TODO Select the comment value in the cell, if present
                        environments[columnIndex - 1].Settings.Add(key, cell);
                    }
                }//for col
            }//for row

            //fixup
            foreach (var environment in environments)
            {
                settingsFile.Environments.Add(environment.EnvironmentName.ToUpperInvariant(), environment);
            }
            return settingsFile;
        }

        #endregion

        #region DATATABLE READER

        /// <summary>
        /// Read the settings from a binary Excel XLS file into a DataTable.
        /// </summary>
        /// <param name="inputFile"></param>
        /// <returns></returns>
        private static DataTable ReadSettingsFromXls(string inputFile)
        {
            DataSet ds = new DataSet();
            OleDbConnection conn = null;

            try
            {
                conn = GetOleDbConnection(inputFile);

                OleDbDataAdapter da = new OleDbDataAdapter("SELECT * FROM [Settings$]", conn);

                try
                {
                    da.Fill(ds);
                }
                catch (OleDbException ex)
                {
                    if (ex.Errors.Count > 0 && ex.Errors[0].NativeError == -537199594)
                    {
                        throw new ArgumentException("ERROR: Could not find a worksheet named Settings.", ex);
                    }

                    throw;
                }
            }
            finally
            {
                if (conn != null)
                {
                    conn.Dispose();
                }
            }

            // We should have gotten back a single DataTable with one or more rows.
            if (ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
            {
                throw new ArgumentException(
                    "ERROR: Could not find a worksheet named Settings or no data on the worksheet.");
            }

            DataTable dt = ds.Tables[0];

            dt.Columns.Add("Comment", typeof(string));

            return dt;
        }

        private static OleDbConnection GetOleDbConnection(string xlsFileName)
        {
            OleDbConnection conn = null;
            string connectionString = null;

            // Try Data Connnectivity Components 2007
            connectionString =
                string.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties=\"Excel 12.0;HDR=NO;MAXSCANROWS=1\"", xlsFileName);

            conn = TryOpenOleDbConnection(connectionString);

            if (conn != null)
            {
                return conn;
            }

            // Try Data Connnectivity Components 2010
            connectionString =
                string.Format("Provider=Microsoft.ACE.OLEDB.14.0;Data Source={0};Extended Properties=\"Excel 14.0;HDR=NO;MAXSCANROWS=1\"", xlsFileName);

            conn = TryOpenOleDbConnection(connectionString);

            if (conn != null)
            {
                return conn;
            }

            // Try old Jet driver if XLS
            if (string.Compare(Path.GetExtension(xlsFileName), ".xls", StringComparison.OrdinalIgnoreCase) == 0)
            {
                connectionString =
                    string.Format("Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};Extended Properties=\"Excel 8.0;HDR=NO;MAXSCANROWS=1\"", xlsFileName);

                conn = TryOpenOleDbConnection(connectionString);

                if (conn != null)
                {
                    return conn;
                }
            }

            throw new Exception(
                "ERROR: Could not open an OLE DB connection to the Excel file. " +
                "Export from XLSX requires installation of Microsoft Office 2007 or 2010 Data Connectivity Components, or JET 4.0 for XLS.");
        }

        private static OleDbConnection TryOpenOleDbConnection(string connectionString)
        {
            OleDbConnection conn = new OleDbConnection(connectionString);

            try
            {
                conn.Open();
            }
            catch (Exception)
            {
                return null;
            }

            return conn;
        }

        #endregion
    }
}
