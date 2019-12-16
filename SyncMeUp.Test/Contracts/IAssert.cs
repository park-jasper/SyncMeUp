namespace SyncMeUp.Test.Contracts
{
    public interface IAssert
    {
        void IsTrue(bool value, string message = "");
        void IsFalse(bool value, string message = "");
        void IsNull(object value, string message = "");
        void IsNotNull(object value, string message = "");
        void AreEqual<T>(T expected, T actual, string message = "");
    }
}