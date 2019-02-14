using System;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;

namespace SyncMeUp.Networking
{
    public abstract class CommunicationBase
    {
        public Guid Guid { get; }
        protected RNGCryptoServiceProvider _randomSource = new RNGCryptoServiceProvider();

        protected CommunicationBase(Guid guid)
        {
            Guid = guid;
        }

        protected Task SendGuid(NetworkStream stream, CancellationToken token)
        {
            var guidByteArray = Guid.ToByteArray();
            return stream.WriteAsync(guidByteArray, 0, guidByteArray.Length, token);
        }

        protected static async Task<NetworkResult<Guid>> GetGuid(NetworkStream stream, CancellationToken token)
        {
            var guidLength = new Guid().ToByteArray().Length;
            byte[] buffer = new byte[guidLength];
            try
            {
                int messageLength = await stream.ReadAsync(buffer, 0, guidLength, token);
                if (messageLength == guidLength)
                {
                    return new NetworkResult<Guid> { Successful = true, Result = new Guid(buffer) };
                }
                else
                {
                    return new NetworkResult<Guid> { Successful = false };
                }
            }
            catch (Exception exc)
            {
                return new NetworkResult<Guid> { Successful = false };
            }
        }

        public static InitiationMode ConvertToInitiationMode(byte[] bytes)
        {
            if (bytes.Length != 4)
            {
                return InitiationMode.Error;
            }
            var value = BitConverter.ToInt32(bytes, 0);
            return (InitiationMode)value;
        }
        public static CommunicationData ConvertToCommunicationData(byte[] bytes)
        {
            if (bytes.Length != 4)
            {
                return CommunicationData.Error;
            }
            var value = BitConverter.ToInt32(bytes, 0);
            return (CommunicationData) value;
        }

        public abstract class CommunicationsControl
        {
            protected CancellationTokenSource TokenSource { get; }
            protected CommunicationsControl(CancellationTokenSource tokenSource)
            {
                TokenSource = tokenSource;
            }

            public virtual void Stop()
            {
                TokenSource.Cancel();
            }
        }
    }
}