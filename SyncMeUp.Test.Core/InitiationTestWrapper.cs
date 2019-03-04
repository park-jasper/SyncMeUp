using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SyncMeUp.Test.Core
{
    [TestClass]
    public class InitiationTestWrapper
    {
        public InitiationTest Test { get; set; }

        public InitiationTestWrapper()
        {
            Test = new InitiationTest(new AssertService());
        }

        [TestMethod]
        public async Task TestOtpInitiation()
        {
            await Test.TestOtpInitiation();
        }
    }
}
