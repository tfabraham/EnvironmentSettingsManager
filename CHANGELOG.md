## Version 1.6.1 (7/5/2011)

Changes in this release:

* FIX: Fix another issue that caused exported files to contain some values from incorrect environments after certain cut/copy/paste operations in the spreadsheet. This is due to Excel sometimes writing out column Index attributes and leaving out some cells in the XML.

NOTE: OLE DB export from XLSX files requires installation of [Office 2007 Data Connectivity Components](https://www.microsoft.com/en-us/download/details.aspx?id=23734) or [Office 2010 Access Runtime](https://www.microsoft.com/en-us/download/details.aspx?id=10910).

## Version 1.6 (7/7/2010)

Changes in this release:

* NEW: Support for Office Data Connectivity Components 2007 and 2010 (XLSX binary format)
* CHANGE: Force EXE to run in 32-bit mode on x64 OS since JET 4.0 OLE DB and Office 2007 Data Connectivity Components only support 32-bit processes
* CHANGE: Rename default template export file names to Exported_X to reinforce that they are auto-generated files
* FIX: Fix issue that caused exported files to contain some values from incorrect environments after cut/copy/paste operations in the spreadsheet (thanks to Ryan Shaw!)

NOTE: OLE DB export from XLSX files requires installation of Office 2007 Data Connectivity Components or Office 2010 Data Connectivity Components/Access Runtime.

## Version 1.5.1 (2/11/2010)

* CHANGE: Fix for Exporter error 'ERROR: Cannot find column X', which could occur when a new environment column was added by copying and pasting an existing column.

## Version 1.5 (1/22/2010)

* NEW - Export to .NET <appSettings> XML format
* NEW - Export to Windows Installer XML (WiX) <include> XML format as a <CustomTable>
* NEW - Command-line switch (/F) to control output format
* CHANGE - Removed most sample values from template spreadsheets
* CHANGE - Updated documentation

## Version 1.0 (2/12/2008)

Initial release of the Environment Settings Manager.
