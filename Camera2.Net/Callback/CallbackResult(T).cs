namespace Camera2.Net.Callback
{
    public class CallbackResult<T>
    {
        public bool Successful { get; set; }
        public T Result { get; set; }
    }

    public class CallbackResult<TResult, TError> : CallbackResult<TResult>
    {
        public TError Error { get; set; }
    }
}