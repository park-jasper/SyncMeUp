using SyncMeUp.Test.Contracts;

namespace SyncMeUp.Test
{
    public class TestBase
    {
        protected IAssert Assert { get; }
        protected TestBase(IAssert assert)
        {
            Assert = assert;
        }
    }
}