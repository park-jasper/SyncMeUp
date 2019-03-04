using System;

namespace SyncMeUp.Domain.Networking
{
    public class NetworkResult
    {
        public bool Successful { get; set; }
        public Exception Exception { get; set; }
    }
    public class NetworkResult<TResult> : NetworkResult
    {
        public TResult Result { get; set; }

        public static NetworkResult<TResult> From(NetworkResult networkResult, TResult result = default(TResult))
        {
            return new NetworkResult<TResult>
            {
                Successful = networkResult.Successful,
                Exception = networkResult.Exception,
                Result = result
            };
        }
    }
}