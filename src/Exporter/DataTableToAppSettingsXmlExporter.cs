// Copyright 2007 Thomas F. Abraham. All Rights Reserved.
// See LICENSE.txt for licensing information.

using System.Xml;

namespace EnvironmentSettingsExporter
{
    /// <summary>
    /// Export the settings contained in a DataTable to multiple XML files, one per declared environment.
    /// The output XML format is the .NET appSettings structure.
    /// </summary>
    internal class DataTableToAppSettingsXmlExporter : DataTableToXmlExporter
    {
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
