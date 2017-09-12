using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System;

namespace EnvSettingsManager
{
    [TestClass]
    public class UnitTest_PipelineFactory
    {
        [TestMethod]
        [DeploymentItem(@"EnvSettingsManager.Test\In\Basic.xls", "DataIn")]
        public void ImporterMake_BinaryFile_CorrectClass()
        {
            string inputFile = "DataIn\\Basic.xls";
            Assert.IsTrue(System.IO.File.Exists(inputFile));

            ImporterBase importer = PipelineFactory.MakeImporter(inputFile);

            Assert.IsNotNull(importer);
            Assert.IsInstanceOfType(importer, typeof(ExcelBinaryImporter));
        }

        [TestMethod]
        [DeploymentItem(@"EnvSettingsManager.Test\In\ApplA.xml", "DataIn")]
        public void ImporterMake_SpreadsheetMLFile_CorrectClass()
        {
            string inputFile = "DataIn\\ApplA.xml";
            Assert.IsTrue(System.IO.File.Exists(inputFile));

            ImporterBase importer = PipelineFactory.MakeImporter(inputFile);

            Assert.IsNotNull(importer);
            Assert.IsInstanceOfType(importer, typeof(SpreadsheetMLImporter));
        }
        
        [TestMethod]
        public void ImporterMake_UnknownFile_Null()
        {
            string inputFile = "Foo.txt";
            File.WriteAllText(inputFile, "");
            Assert.IsTrue(System.IO.File.Exists(inputFile));

            ImporterBase importer = PipelineFactory.MakeImporter(inputFile);

            Assert.IsNull(importer);
        }

        [TestMethod]
        [ExpectedException(typeof(FileNotFoundException))]
        public void ImporterMake_FileNotFound_Exception()
        {
            string inputFile = "Bar.txt";

            ImporterBase importer = PipelineFactory.MakeImporter(inputFile);

            Assert.IsNull(importer);
        }

        [TestMethod]
        public void ProcessorMake_Export_Succeed()
        {
            ProcessorBase processor = PipelineFactory.MakeProcessor(ProgramAction.Export);
            Assert.IsInstanceOfType(processor, typeof(NullProcessor));
        }

        [TestMethod]
        public void ProcessorMake_Merge_Succeed()
        {
            ProcessorBase processor = PipelineFactory.MakeProcessor(ProgramAction.Merge);
            Assert.IsInstanceOfType(processor, typeof(MergeProcessor));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ProcessorMake_InvalidValue_Null()
        {
            ProcessorBase processor = PipelineFactory.MakeProcessor((ProgramAction)2);
        }

        [TestMethod]
        public void ExporterMake_Export_AppSettings()
        {
            var args = new ExportActionArguments()
            {
                action = ProgramAction.Export,
                format = FormatType.AppSettings
            };
            ExporterBase exporter = PipelineFactory.MakeExporter(args);
            Assert.IsNotNull(exporter);
            Assert.IsInstanceOfType(exporter, typeof(AppSettingsExporter));
        }
    }
}
