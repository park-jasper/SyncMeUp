namespace SyncMeUp.Networking
{
    public class NetworkResult<T>
    {
        public bool Successful { get; set; }
        public T Result { get; set; }
    }
}