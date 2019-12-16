using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SyncMeUp.Test.Domain;

namespace SyncMeUp.Test.Core
{
    [TestClass]
    public class ContainerScannerTestWrapper
    {
        public ContainerScannerTest Test { get; set; }

        public ContainerScannerTestWrapper()
        {
            Test = new ContainerScannerTest(new AssertService());
        }

        [TestMethod]
        public async Task TestFileScanning()
        {
            await Test.TestFileScanning();
        }

        [TestMethod]
        public async Task TestNestedFolderScanAllNewFiles()
        {
            await Test.TestNestedFolderScanAllNewFiles();
        }
    }
}