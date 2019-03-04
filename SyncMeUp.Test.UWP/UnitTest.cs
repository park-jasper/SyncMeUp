
using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SyncMeUp.Test.UWP
{
    [TestClass]
    public class InitiationTestWrapper
    {
        public InitiationTest Test { get; }

        public InitiationTestWrapper()
        {
            Test = new InitiationTest();
        }

        [TestMethod]
        public async Task TestOtpInitiation()
        {
            await Test.TestOtpInitiation();
        }
    }
}
