using Microsoft.VisualStudio.TestTools.UnitTesting;
using SyncMeUp.Test.Contracts;

namespace SyncMeUp.Test.Core
{
    public class AssertService : IAssert
    {
        //if (string.IsNullOrEmpty(message))
        //{

        //}
        //else
        //{

        //}

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

        public void IsFalse(bool value, string message = "")
        {
            if (string.IsNullOrEmpty(message))
            {
                Assert.IsFalse(value);
            }
            else
            {
                Assert.IsFalse(value, message);
            }
        }

        public void IsNull(object value, string message = "")
        {
            if (string.IsNullOrEmpty(message))
            {
                Assert.IsNull(value);
            }
            else
            {
                Assert.IsNull(value, message);
            }
        }

        public void IsNotNull(object value, string message = "")
        {
            if (string.IsNullOrEmpty(message))
            {
                Assert.IsNotNull(value);
            }
            else
            {
                Assert.IsNotNull(value, message);
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