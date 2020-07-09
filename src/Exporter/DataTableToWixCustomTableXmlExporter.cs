// Copyright 2007 Thomas F. Abraham. All Rights Reserved.
// See LICENSE.txt for licensing information.

using System.Xml;

namespace EnvironmentSettingsExporter
{
    /// <summary>
    /// Export the settings contained in a DataTable to a single XML file.
    /// The output format is a WiX (Windows Installer XML) include file containing a custom table.
    /// </summary>
    internal class DataTableToWixCustomTableXmlExporter : DataTableToXmlExporter
    {
        private int index = 1;

        protected override void WriteHeader(XmlWriter xmlw, string environmentName)
        {
            xmlw.WriteStartElement("include");
            
            xmlw.WriteStartElement("CustomTable");
            xmlw.WriteAttributeString("Id", "EnvironmentSettings");

            xmlw.WriteStartElement("Column");
            xmlw.WriteAttributeString("Id", "Id");
            xmlw.WriteAttributeString("Category", "Identifier");
            xmlw.WriteAttributeString("PrimaryKey", "yes");
            xmlw.WriteAttributeString("Type", "int");
            xmlw.WriteAttributeString("Width", "4");
            xmlw.WriteEndElement();

            xmlw.WriteStartElement("Column");
            xmlw.WriteAttributeString("Id", "Environment");
            xmlw.WriteAttributeString("Category", "Text");
            xmlw.WriteAttributeString("Type", "string");
            xmlw.WriteAttributeString("PrimaryKey", "no");
            xmlw.WriteEndElement();

            xmlw.WriteStartElement("Column");
            xmlw.WriteAttributeString("Id", "Key");
            xmlw.WriteAttributeString("Category", "Text");
            xmlw.WriteAttributeString("Type", "string");
            xmlw.WriteAttributeString("PrimaryKey", "no");
            xmlw.WriteEndElement();

            xmlw.WriteStartElement("Column");
            xmlw.WriteAttributeString("Id", "Value");
            xmlw.WriteAttributeString("Category", "Text");
            xmlw.WriteAttributeString("Type", "string");
            xmlw.WriteAttributeString("PrimaryKey", "no");
            xmlw.WriteAttributeString("Nullable", "yes");
            xmlw.WriteEndElement();
        }

        protected override void WriteValue(XmlWriter xmlw, string settingName, string settingValue, string environmentName, bool valueContainsReservedXmlCharacter)
        {
            xmlw.WriteStartElement("Row");

            xmlw.WriteStartElement("Data");
            xmlw.WriteAttributeString("Column", "Id");
            xmlw.WriteValue(index);
            xmlw.WriteEndElement();

            xmlw.WriteStartElement("Data");
            xmlw.WriteAttributeString("Column", "Environment");
            xmlw.WriteValue(environmentName);
            xmlw.WriteEndElement();

            xmlw.WriteStartElement("Data");
            xmlw.WriteAttributeString("Column", "Key");
            xmlw.WriteValue(settingName);
            xmlw.WriteEndElement();

            xmlw.WriteStartElement("Data");
            xmlw.WriteAttributeString("Column", "Value");
            xmlw.WriteValue(settingValue);
            xmlw.WriteEndElement();

            xmlw.WriteEndElement();

            index++;
        }

        protected override void WriteFooter(XmlWriter xmlw, string environmentName)
        {
            xmlw.WriteEndElement();
            xmlw.WriteEndElement();
        }
    }
}
