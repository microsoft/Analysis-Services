using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using Amo=Microsoft.AnalysisServices;
using Tom=Microsoft.AnalysisServices.Tabular;

namespace BismNormalizer.Tests
{
    [TestClass]
    public class BismNormalizerTests
    {
        [ClassInitialize]
        public static void InitializeDbs(TestContext testContext)
        {
            ExecBatFile("CreateDbsRunBSMN.bat");
        }

        [ClassCleanup]
        public static void CleanupDbs()
        {
            ExecBatFile("DeleteDbs.bat");
        }

        [TestMethod]
        public void TableCount1103()
        {
            using (Amo.Server server = new Amo.Server())
            {
                server.Connect("localhost\\tb");

                Amo.Database db = server.Databases.FindByName("Test1103_Target");
                Assert.IsNotNull(db);

                Assert.AreEqual(3, db.Cubes[0].Dimensions.Count);
                server.Disconnect();
            }
        }

        [TestMethod]
        public void TableCount1200()
        {
            using (Tom.Server server = new Tom.Server())
            {
                server.Connect("localhost\\tb");

                Tom.Database db = server.Databases.FindByName("Test1200_Target");
                Assert.IsNotNull(db);

                Assert.AreEqual(6, db.Model.Tables.Count);
                server.Disconnect();
            }
        }

        private static void ExecBatFile(string batFileName)
        {
            Assert.IsTrue(File.Exists(batFileName));
            Process proc = new Process();
            proc.StartInfo.UseShellExecute = false;
            proc.StartInfo.RedirectStandardOutput = true;
            proc.StartInfo.FileName = batFileName;
            proc.Start();
            Console.WriteLine(proc.StandardOutput.ReadToEnd());
            proc.WaitForExit();
            Assert.AreEqual(proc.ExitCode, 0);
        }
    }
}
