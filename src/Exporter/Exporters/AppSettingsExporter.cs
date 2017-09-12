// (c) Copyright 2007-10 Thomas F. Abraham.
// This source is subject to the Microsoft Public License (Ms-PL).
// See http://www.opensource.org/licenses/ms-pl.html
// All other rights reserved.

using System.Xml;

namespace EnvSettingsManager
{
    /// <summary>
    /// Export the settings contained in a DataTable to multiple XML files, one per declared environment.
    /// The output XML format is the .NET appSettings structure.
    /// </summary>
    internal class AppSettingsExporter : XmlExporterBase
    {
        internal AppSettingsExporter(ExportActionArguments args)
            : base(args)
        { }

        protected override void WriteHeader(XmlWriter xmlw, string environmentName)
        {
            xmlw.WriteStartElement("appSettings");
        }

        protected override void WriteValue(XmlWriter xmlw, string settingName, string settingValue, string environmentName, bool valueContainsReservedXmlCharacter)
        {
            xmlw.WriteStartElement("add");
            xmlw.WriteAttributeString("key", settingName);

            // Write the value to the XML stream
            xmlw.WriteAttributeString("value", settingValue);

            xmlw.WriteEndElement();
        }

        protected override void WriteFooter(XmlWriter xmlw, string environmentName)
        {
            xmlw.WriteEndElement();
        }
    }
}
