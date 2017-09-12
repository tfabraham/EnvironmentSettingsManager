// (c) Copyright 2007-10 Thomas F. Abraham.
// This source is subject to the Microsoft Public License (Ms-PL).
// See http://www.opensource.org/licenses/ms-pl.html
// All other rights reserved.

using System.Xml;

namespace EnvSettingsManager
{
    /// <summary>
    /// Export the settings contained in a DataTable to multiple XML files, one per declared environment.
    /// The output format is the XmlPreprocess settings file format.
    /// </summary>
    internal class XmlPreprocessExporter : XmlExporterBase
    {
        internal XmlPreprocessExporter(ExportActionArguments args)
            : base(args)
        { }

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
