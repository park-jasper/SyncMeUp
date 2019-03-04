using Microsoft.VisualStudio.TestTools.UnitTesting;
using SyncMeUp.Test.Contracts;

namespace SyncMeUp.Test.Core
{
    public class AssertService : IAssert
    {
        public void IsTrue(bool value, string message = "")
        {
            if (string.IsNullOrEmpty(message))
            {
                Assert.IsTrue(value);
            }
            else
            {
                Assert.IsTrue(value, message);
            }
        }
        public void AreEqual<T>(T expected, T actual, string message = "")
        {
            if (string.IsNullOrEmpty(message))
            {
                Assert.AreEqual(expected, actual);
            }
            else
            {
                Assert.AreEqual(expected, actual, message);
            }
        }
        
    }
}