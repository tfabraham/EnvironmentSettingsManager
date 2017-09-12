// (c) Copyright 2007-10 Thomas F. Abraham.
// This source is subject to the Microsoft Public License (Ms-PL).
// See http://www.opensource.org/licenses/ms-pl.html
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Text;

namespace EnvironmentSettingsExporter
{
    /// <summary>
    /// Reads a binary Excel workbook file.
    /// </summary>
    internal static class ExcelBinarySettingsFileReader
    {
        /// <summary>
        /// Read the settings from a binary Excel XLS file into a DataTable.
        /// </summary>
        /// <param name="inputFile"></param>
        /// <returns></returns>
        internal static DataTable ReadSettingsFromXls(string inputFile)
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
    }
}
