using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EnvSettingsManager
{
    [TestClass]
    [DeploymentItem(@"EnvSettingsManager.Test\In\Basic.xls", "DataIn")]
    public class IntegrationTest1
    {
        [TestMethod]
        public void Commandline_Empty_Return42()
        {
            // no args -> help
            int rc = Program.Main(new string[] { });
            Assert.AreEqual(42, rc);
        }

        [TestMethod]
        public void Commandline_ValidOptionNoAction_Return42()
        {
            int rc = Program.Main(new string[] { "", @"/i:whatever" });
            Assert.AreEqual(42, rc);
        }

        [TestMethod]
        public void Commandline_ValidActionNoOptions_Return42()
        {
            int rc = Program.Main(new string[] { @"Export" });
            Assert.AreEqual(42, rc);
        }

        [TestMethod]
        public void Commandline_InvalidActionNoOptions_Return42()
        {
            int rc = Program.Main(new string[] { @"What" });
            Assert.AreEqual(42, rc);
        }

        [TestMethod]
        public void Commandline_ExportXmlPreprocess_Succeed()
        {
            int rc = Program.Main(new string[] { "Export", @"/i:DataIn\Basic.xls", @"/o:Result", "/f:XmlPreprocess" });
            Assert.AreEqual(0, rc);
        }

        [TestMethod]
        [DeploymentItem(@"EnvSettingsManager.Test\Out\Basic\Exported_LocalSettings.xml", "Expected")]
        [DeploymentItem(@"EnvSettingsManager.Test\Out\Basic\Exported_DevSettings.xml", "Expected")]
        [DeploymentItem(@"EnvSettingsManager.Test\Out\Basic\Exported_TestSettings.xml", "Expected")]
        [DeploymentItem(@"EnvSettingsManager.Test\Out\Basic\Exported_ProdSettings.xml", "Expected")]
        public void Export_BasicXLS_EqualsExpected()
        {
            int rc = Program.Main(new string[] { "Export", @"/i:DataIn\Basic.xls", @"/o:Exports", "/f:XmlPreprocess" });
            Assert.AreEqual(0, rc);
            Assert.IsTrue(Utilities.AreXmlFilesEqual(@"Expected\Exported_LocalSettings.xml", @"Exports\Exported_LocalSettings.xml"));
            Assert.IsTrue(Utilities.AreXmlFilesEqual(@"Expected\Exported_DevSettings.xml", @"Exports\Exported_DevSettings.xml"));
            Assert.IsTrue(Utilities.AreXmlFilesEqual(@"Expected\Exported_TestSettings.xml", @"Exports\Exported_TestSettings.xml"));
            Assert.IsTrue(Utilities.AreXmlFilesEqual(@"Expected\Exported_ProdSettings.xml", @"Exports\Exported_ProdSettings.xml"));
        }

        [TestMethod]
        [DeploymentItem(@"EnvSettingsManager.Test\In\Basic.xls", "DataIn")]
        [DeploymentItem(@"EnvSettingsManager.Test\Out\Merge\OneRowFile.xml", "Expected")]
        public void Merge_OneFiles_EqualsExpected()
        {
            int rc = Program.Main(new string[] { "Merge", @"/i:DataIn\Basic.xls", @"/o:Merge1.xml" });
            Assert.AreEqual(0, rc);
            Assert.IsTrue(Utilities.AreXmlFilesEqual(@"Expected\OneRowFile.xml", @"Merge1.xml"));
        }

        [TestMethod]
        [DeploymentItem(@"EnvSettingsManager.Test\In\Basic.xls", "DataIn")]
        [DeploymentItem(@"EnvSettingsManager.Test\Out\Merge\OneRowFile.xml", "Expected")]
        public void Merge_SameFileTwice_EqualsExpected()
        {
            int rc = Program.Main(new string[] { "Merge", @"/i:DataIn\Basic.xls", @"/i:DataIn\Basic.xls", @"/o:Merge1.xml" });
            Assert.AreEqual(0, rc);
            Assert.IsTrue(Utilities.AreXmlFilesEqual(@"Expected\OneRowFile.xml", @"Merge1.xml"));
        }

        [TestMethod]
        [DeploymentItem(@"EnvSettingsManager.Test\In\ApplA.xml", "DataIn")]
        [DeploymentItem(@"EnvSettingsManager.Test\In\ApplB.xml", "DataIn")]
        [DeploymentItem(@"EnvSettingsManager.Test\Out\Merge\TwoDifforms-Unsorted.xml", "Expected")]
        public void Merge_TwoDifformFiles_EqualsExpected()
        {
            int rc = Program.Main(new string[] { "Merge", @"/i:DataIn\ApplA.xml", @"/i:DataIn\ApplB.xml", @"/o:Merge2.xml" });
            Assert.AreEqual(0, rc);
            Assert.IsTrue(Utilities.AreXmlFilesEqual(@"Expected\TwoDifforms-Unsorted.xml", @"Merge2.xml"));
        }

        [TestMethod]
        [DeploymentItem(@"EnvSettingsManager.Test\In\ApplA.xml", "DataIn")]
        [DeploymentItem(@"EnvSettingsManager.Test\In\ApplC.xml", "DataIn")]
        [DeploymentItem(@"EnvSettingsManager.Test\Out\Merge\FirstWins.xml", "Expected")]
        public void Merge_FirstWins_EqualsExpected()
        {
            int rc = Program.Main(new string[] { "Merge", @"/i:DataIn\ApplA.xml", @"/i:DataIn\ApplC.xml", @"/o:Merge2.xml", "/raiseErrorOnConflict-" });
            Assert.AreEqual(0, rc);
            Assert.IsTrue(Utilities.AreXmlFilesEqual(@"Expected\FirstWins.xml", @"Merge2.xml"));
        }

        [TestMethod]
        [DeploymentItem(@"EnvSettingsManager.Test\In\ApplA.xml", "DataIn")]
        [DeploymentItem(@"EnvSettingsManager.Test\In\ApplC.xml", "DataIn")]
        [DeploymentItem(@"EnvSettingsManager.Test\Out\Merge\LastWins.xml", "Expected")]
        public void Merge_LastWins_EqualsExpected()
        {
            int rc = Program.Main(new string[] { "Merge", @"/i:DataIn\ApplA.xml", @"/i:DataIn\ApplC.xml", @"/o:Merge2.xml", "/raiseErrorOnConflict-", "/lastWins+" });
            Assert.AreEqual(0, rc);
            Assert.IsTrue(Utilities.AreXmlFilesEqual(@"Expected\LastWins.xml", @"Merge2.xml"));
        }
    }
}
