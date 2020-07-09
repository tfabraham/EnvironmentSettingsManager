// Copyright 2007 Thomas F. Abraham. All Rights Reserved.
// See LICENSE.txt for licensing information.

using System.Xml;

namespace EnvironmentSettingsExporter
{
    /// <summary>
    /// Export the settings contained in a DataTable to multiple XML files, one per declared environment.
    /// The output format is the XmlPreprocess settings file format.
    /// </summary>
    internal class DataTableToXmlPreprocessXmlExporter : DataTableToXmlExporter
    {
        protected override void WriteHeader(XmlWriter xmlw, string environmentName)
        {
            xmlw.WriteStartElement("settings");
        }

        protected override void WriteValue(XmlWriter xmlw, string settingName, string settingValue, string environmentName, bool valueContainsReservedXmlCharacter)
        {
            xmlw.WriteStartElement("property");

            xmlw.WriteAttributeString("name", settingName);

            // Write the value to the XML stream
            if (valueContainsReservedXmlCharacter)
            {
                xmlw.WriteCData(settingValue);
            }
            else
            {
                xmlw.WriteString(settingValue);
            }

            xmlw.WriteEndElement();
        }

        protected override void WriteFooter(XmlWriter xmlw, string environmentName)
        {
            xmlw.WriteEndElement();
        }
    }
}
