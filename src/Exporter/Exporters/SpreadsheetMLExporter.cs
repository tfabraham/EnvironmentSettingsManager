using System.Collections.Generic;
using System.Reflection;
using System.Xml;

namespace EnvSettingsManager
{
    internal class SpreadsheetMLExporter : ExporterBase
    {        
        const string ss = SpreadsheetMLConstants.ss;

        internal override void ExportSettings(List<SettingsFile> settingsFiles, string outputPath)
        {
            //TODO check 1
            ExportSettings(settingsFiles[0], outputPath);
        }

        internal virtual void ExportSettings(SettingsFile settingsFile, string outputFileName)
        {
            WriteFile(settingsFile, outputFileName);
        }

        #region WRITING

        internal static void WriteFile(SettingsFile settingsFile, string filename)
        {
            var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("EnvSettingsManager.Exporters.EnvironmentSettingsTemplate.xml");
            var doc = new XmlDocument();
            var nm = new XmlNamespaceManager(doc.NameTable);
            nm.AddNamespace("ss", "urn:schemas-microsoft-com:office:spreadsheet");
            doc.Load(stream);
            var table = doc.SelectSingleNode("//ss:Table[comment()[.='PLACEHOLDER']]", nm) as XmlElement;

            WriteHeader(settingsFile, table);
            WriteData(settingsFile, table);

            doc.Save(filename);
        }

        private static void WriteHeader(SettingsFile settingsFile, XmlElement table)
        {
            int numEnvironments = settingsFile.Environments.Count;
            int numSettings = settingsFile.Settings.Count;
            table.SetAttribute("ExpandedColumnCount", ss, (numEnvironments + 1).ToString());
            table.SetAttribute("ExpandedRowCount", ss, (10 + numSettings).ToString());

            WriteFirstRow(table, numEnvironments);
            WriteSecondRow(settingsFile, table);
            WriteThirdRow(settingsFile, table);
            WriteFourthRow(settingsFile, table);
            WriteFifthRow(table, numEnvironments);
            WriteSixtRow(table, numEnvironments);
        }

        private static void WriteFirstRow(XmlElement table, int numEnvironments)
        {
            XmlElement cell, data;
            /*
               <Row ss:AutoFitHeight="0" ss:Height="18" ss:StyleID="s64">
                <Cell ss:StyleID="s77"><Data ss:Type="String">Environment Settings</Data></Cell>
                <Cell ss:StyleID="s65"/>
                <Cell ss:StyleID="s65"/>
                <Cell ss:StyleID="s65"/>
                <Cell ss:StyleID="s65"/>
                <Cell ss:StyleID="s65"/>
               </Row>
             */
            var firstRow = table.OwnerDocument.CreateElement("ss:Row", ss);
            firstRow.SetAttribute("AutoFitHeight", ss, "0");
            firstRow.SetAttribute("Height", ss, "18");
            firstRow.SetAttribute("StyleID", ss, "s64");
            cell = table.OwnerDocument.CreateElement("ss:Cell", ss);
            cell.SetAttribute("StyleID", ss, "s77");
            firstRow.AppendChild(cell);
            data = table.OwnerDocument.CreateElement("ss:Data", ss);
            data.SetAttribute("Type", ss, "String");
            data.InnerText = "Environment Settings";
            cell.AppendChild(data);

            for (int i = 0; i < numEnvironments; i++)
            {
                cell = table.OwnerDocument.CreateElement("ss:Cell", ss);
                cell.SetAttribute("StyleID", ss, "s65");
                firstRow.AppendChild(cell);
            }
            table.AppendChild(firstRow);
        }

        private static void WriteSecondRow(SettingsFile settingsFile, XmlElement table)
        {
            XmlElement cell, data;
            /*
               <Row>
                <Cell ss:StyleID="s65"><Data ss:Type="String">Environment Name:</Data></Cell>
                <Cell ss:StyleID="s65"><Data ss:Type="String">Default Values</Data></Cell>
                <Cell ss:StyleID="s78"><Data ss:Type="String">Local Development</Data></Cell>
                <Cell ss:StyleID="s78"><Data ss:Type="String">Shared Development</Data></Cell>
                <Cell ss:StyleID="s78"><Data ss:Type="String">QA</Data></Cell>
                <Cell ss:StyleID="s78"><Data ss:Type="String">Production</Data></Cell>
               </Row>
             */
            var secondRow = table.OwnerDocument.CreateElement("ss:Row", ss);
            cell = table.OwnerDocument.CreateElement("ss:Cell", ss);
            cell.SetAttribute("StyleID", ss, "s65");
            secondRow.AppendChild(cell);
            data = table.OwnerDocument.CreateElement("ss:Data", ss);
            data.SetAttribute("Type", ss, "String");
            data.InnerText = "Environment Name:";
            cell.AppendChild(data);

            foreach (var env in settingsFile.Environments)
            {
                cell = table.OwnerDocument.CreateElement("ss:Cell", ss);
                if (env.Key == SettingsFile.DefaultValueEnvironmentName)
                    cell.SetAttribute("StyleID", ss, "s65");
                else
                    cell.SetAttribute("StyleID", ss, "s78");
                data = table.OwnerDocument.CreateElement("ss:Data", ss);
                data.SetAttribute("Type", ss, "String");
                data.InnerText = env.Value.EnvironmentName;
                cell.AppendChild(data);
                secondRow.AppendChild(cell);
            }
            table.AppendChild(secondRow);
        }

        private static void WriteThirdRow(SettingsFile settingsFile, XmlElement table)
        {
            XmlElement cell, data;
            /*
               <Row>
                <Cell ss:StyleID="s65"><Data ss:Type="String">Generate File?</Data></Cell>
                <Cell ss:StyleID="s66"/>
                <Cell ss:StyleID="s79"><Data ss:Type="String">Yes</Data></Cell>
                <Cell ss:StyleID="s79"><Data ss:Type="String">Yes</Data></Cell>
                <Cell ss:StyleID="s79"><Data ss:Type="String">Yes</Data></Cell>
                <Cell ss:StyleID="s79"><Data ss:Type="String">Yes</Data></Cell>
                <Cell ss:StyleID="s68"/>
               </Row>
             */
            var thirdRow = table.OwnerDocument.CreateElement("ss:Row", ss);
            cell = table.OwnerDocument.CreateElement("ss:Cell", ss);
            cell.SetAttribute("StyleID", ss, "s65");
            thirdRow.AppendChild(cell);
            data = table.OwnerDocument.CreateElement("ss:Data", ss);
            data.SetAttribute("Type", ss, "String");
            data.InnerText = "Generate File?";
            cell.AppendChild(data);

            foreach (var env in settingsFile.Environments)
            {
                cell = table.OwnerDocument.CreateElement("ss:Cell", ss);
                if (env.Key == SettingsFile.DefaultValueEnvironmentName)
                {
                    cell.SetAttribute("StyleID", ss, "s66");
                }
                else
                {
                    if (env.Value.GenerateFile)
                    {
                        cell.SetAttribute("StyleID", ss, "s79");
                        data = table.OwnerDocument.CreateElement("ss:Data", ss);
                        data.SetAttribute("Type", ss, "String");
                        data.InnerText = "Yes";
                        cell.AppendChild(data);
                    }
                    else
                    {
                        cell.SetAttribute("StyleID", ss, "s79");
                        data = table.OwnerDocument.CreateElement("ss:Data", ss);
                        data.SetAttribute("Type", ss, "String");
                        data.InnerText = "No";
                        cell.AppendChild(data);
                    }
                }
                thirdRow.AppendChild(cell);
            }
            table.AppendChild(thirdRow);
        }

        private static void WriteFourthRow(SettingsFile settingsFile, XmlElement table)
        {
            XmlElement cell, data;
            /*
               <Row>
                <Cell ss:StyleID="s65"><Data ss:Type="String">Settings File Name:</Data></Cell>
                <Cell ss:StyleID="s69"><Data ss:Type="String">Put values here that apply to all</Data></Cell>
                <Cell ss:StyleID="s79"><Data ss:Type="String">local_settings.xml</Data></Cell>
                <Cell ss:StyleID="s79"><Data ss:Type="String">DEV_settings.xml</Data></Cell>
                <Cell ss:StyleID="s80"><Data ss:Type="String">QA_settings.xml</Data></Cell>
                <Cell ss:StyleID="s80"><Data ss:Type="String">PROD_settings.xml</Data></Cell>
                <Cell ss:StyleID="s71"/>
               </Row>
             */
            var fourthRow = table.OwnerDocument.CreateElement("ss:Row", ss);
            cell = table.OwnerDocument.CreateElement("ss:Cell", ss);
            cell.SetAttribute("StyleID", ss, "s65");
            fourthRow.AppendChild(cell);
            data = table.OwnerDocument.CreateElement("ss:Data", ss);
            data.SetAttribute("Type", ss, "String");
            data.InnerText = "Settings File Name:";
            cell.AppendChild(data);

            foreach (var env in settingsFile.Environments)
            {
                cell = table.OwnerDocument.CreateElement("ss:Cell", ss);
                if (env.Key == SettingsFile.DefaultValueEnvironmentName)
                {
                    cell.SetAttribute("StyleID", ss, "s69");
                    data = table.OwnerDocument.CreateElement("ss:Data", ss);
                    data.SetAttribute("Type", ss, "String");
                    data.InnerText = "Put values here that apply to all";
                    cell.AppendChild(data);
                }
                else
                {
                    cell.SetAttribute("StyleID", ss, "s79");
                    data = table.OwnerDocument.CreateElement("ss:Data", ss);
                    data.SetAttribute("Type", ss, "String");
                    data.InnerText = env.Value.Filename;
                    cell.AppendChild(data);
                }
                fourthRow.AppendChild(cell);
            }
            table.AppendChild(fourthRow);
        }

        private static void WriteFifthRow(XmlElement table, int numEnvironments)
        {
            XmlElement cell, data;
            /*
               <Row>
                <Cell ss:StyleID="s65"/>
                <Cell ss:StyleID="s69"><Data ss:Type="String">environments by default.</Data></Cell>
                <Cell ss:StyleID="s67"/>
                <Cell ss:StyleID="s67"/>
                <Cell ss:StyleID="s70"/>
                <Cell ss:StyleID="s70"/>
                <Cell ss:StyleID="s71"/>
               </Row>
             */
            var fifthRow = table.OwnerDocument.CreateElement("ss:Row", ss);
            cell = table.OwnerDocument.CreateElement("ss:Cell", ss);
            cell.SetAttribute("StyleID", ss, "s65");
            fifthRow.AppendChild(cell);
            
            cell = table.OwnerDocument.CreateElement("ss:Cell", ss);
            cell.SetAttribute("StyleID", ss, "s69");
            fifthRow.AppendChild(cell);
            data = table.OwnerDocument.CreateElement("ss:Data", ss);
            data.SetAttribute("Type", ss, "String");
            data.InnerText = "environments by default.";
            cell.AppendChild(data);


            for (int i = 1; i < numEnvironments; i++)
            {
                cell = table.OwnerDocument.CreateElement("ss:Cell", ss);
                cell.SetAttribute("StyleID", ss, "s67");
                fifthRow.AppendChild(cell);
            }
            table.AppendChild(fifthRow);
        }

        private static void WriteSixtRow(XmlElement table, int numEnvironments)
        {
            XmlElement cell, data;
            /*
               <Row>
                <Cell ss:StyleID="s65"><Data ss:Type="String">Settings:</Data></Cell>
                <Cell ss:StyleID="s69"/>
                <Cell ss:StyleID="s67"/>
                <Cell ss:StyleID="s67"/>
                <Cell ss:StyleID="s70"/>
                <Cell ss:StyleID="s70"/>
                <Cell ss:StyleID="s71"/>
               </Row>
             */
            var sixtRow = table.OwnerDocument.CreateElement("ss:Row", ss);
            cell = table.OwnerDocument.CreateElement("ss:Cell", ss);
            cell.SetAttribute("StyleID", ss, "s65");
            sixtRow.AppendChild(cell);
            data = table.OwnerDocument.CreateElement("ss:Data", ss);
            data.SetAttribute("Type", ss, "String");
            data.InnerText = "Settings:";
            cell.AppendChild(data);

            for (int i = 0; i < numEnvironments; i++)
            {
                cell = table.OwnerDocument.CreateElement("ss:Cell", ss);
                cell.SetAttribute("StyleID", ss, "s67");
                sixtRow.AppendChild(cell);
            }
            table.AppendChild(sixtRow);
        }

        private static void WriteData(SettingsFile settingsFile, XmlElement table)
        {
            XmlElement cell, data;
            /*
               <Row>
                <Cell ss:StyleID="s72"><Data ss:Type="String">SampleSetting</Data><Comment
                  ss:Author="Thomas Abraham"><ss:Data
                   xmlns="http://www.w3.org/TR/REC-html40"><Font html:Face="Tahoma"
                    x:CharSet="1" html:Size="8" html:Color="#000000">Cell comments will be exported to the output XML files</Font></ss:Data></Comment></Cell>
                <Cell ss:StyleID="Default"/>
                <Cell ss:StyleID="Default"><Data ss:Type="String">LocalDevValue</Data></Cell>
                <Cell ss:StyleID="Default"><Data ss:Type="String">SharedDevValue</Data></Cell>
                <Cell ss:StyleID="Default"><Data ss:Type="String">QAValue</Data></Cell>
                <Cell ss:StyleID="Default"><Data ss:Type="String">ProductionValue</Data></Cell>
               </Row>
            */
            var allKeys = settingsFile.Settings.Keys;

            foreach (var key in allKeys)
            {
                var row = table.OwnerDocument.CreateElement("ss:Row", ss);

                Setting setting = settingsFile.Settings[key];

                // setting name
                cell = table.OwnerDocument.CreateElement("ss:Cell", ss);
                cell.SetAttribute("StyleID", ss, "s72");
                row.AppendChild(cell);
                data = table.OwnerDocument.CreateElement("ss:Data", ss);
                data.SetAttribute("Type", ss, "String");
                data.InnerText = setting.Name;
                cell.AppendChild(data);
                // setting comment
                if (!string.IsNullOrEmpty(setting.Comment))
                {
                    var comment = table.OwnerDocument.CreateElement("ss:Comment", ss);
                    data = table.OwnerDocument.CreateElement("ss:Data", ss);
                    data.InnerText = setting.Comment;
                    comment.AppendChild(data);
                    cell.AppendChild(comment);
                }

                foreach (var env in settingsFile.Environments)
                {
                    // cell is added anyway
                    cell = table.OwnerDocument.CreateElement("ss:Cell", ss);
                    cell.SetAttribute("StyleID", ss, "Default");
                    row.AppendChild(cell);
                    if (env.Value.Settings.ContainsKey(key))
                    {
                        //data only if we have a value
                        var value = env.Value.Settings[key];
                        data = table.OwnerDocument.CreateElement("ss:Data", ss);
                        data.SetAttribute("Type", ss, "String");
                        data.InnerText = value.Value;
                        cell.AppendChild(data);
                        // setting's value comment
                        if (!string.IsNullOrEmpty(value.Comment))
                        {
                            var comment = table.OwnerDocument.CreateElement("ss:Comment", ss);
                            data = table.OwnerDocument.CreateElement("ss:Data", ss);
                            data.InnerText = value.Comment;
                            comment.AppendChild(data);
                            cell.AppendChild(comment);
                        }
                    }
                }//for

                table.AppendChild(row);
            }//for
        }

        #endregion

    }
}
