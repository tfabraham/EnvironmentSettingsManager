# Environment Settings Manager
Easy-to-use configuration management tools for deployments with config settings that vary per environment (dev, test, prod, etc.)

The best way to introduce the Environment Settings Manager is to explain the motivations behind its creation:
1. Most software projects have dynamic configuration settings, and their values usually vary per deployment environment (development, test, production, etc.).
1. Maintaining the values of many settings across many environments is a difficult, error-prone task.
1. Configuration settings and their values must be understood and communicated across departmental boundaries, for instance from development to IT support.
1. It is common to store configuration settings in XML, whether they exist as an XML file in the file system or as XML stored in a database column.
1. Configuration settings must be synchronized with ever-changing program code, usually in a source control/versioning tool.

The Environment Settings Manager consists of an Excel spreadsheet and an associated command-line export utility.  Here’s how the Environment Settings Manager’s spreadsheet and exporter can help with the issues described above:
1. Settings are maintained with a well-known, user-friendly tool – Microsoft Excel 2003 or newer.
1. The spreadsheet can store hundreds of settings and their values for dozens of environments in an easy-to-read tabular format.
1. Cell comments allow descriptions to be entered along with the setting name, which can provide helpful context and meaning to the setting name itself.
1. Settings may be saved in either XML or native Excel binary format – the editing experience in Excel is identical either way.
1. The settings spreadsheet (in XML or binary format) can be easily shared between interested parties.
1. Excel’s cell locking allows selected setting values or names to be easily protected from editing by other users.
1. The command-line exporter tool makes it easy to export the settings and their values to a number of different XML formats, ready for consumption by a .NET configuration file, Windows Installer XML (WiX) project or Loren Halvorson's [XmlPreprocess](https://github.com/lorenh/xmlpreprocess) tool.

The Environment Settings Manager is also integrated into the [Deployment Framework for BizTalk](https://github.com/BTDF).

## License

Copyright (c) Thomas F. Abraham. All rights reserved.

Licensed under the [MIT](LICENSE.txt) License.
