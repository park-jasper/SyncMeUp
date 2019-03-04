using System;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using SyncMeUp.Domain.Contracts;

namespace SyncMeUp.Domain.Services
{
    public class RealNetworkStream : INetworkStream, IDisposable
    {
        private readonly NetworkStream _stream;
        public RealNetworkStream(NetworkStream stream)
        {
            _stream = stream;
        }
        public Task<int> ReadAsync(byte[] buffer, int count, CancellationToken token)
        {
            return _stream.ReadAsync(buffer, 0, count, token);
        }

        public Task WriteAsync(byte[] buffer, int count, CancellationToken token)
        {
            return _stream.WriteAsync(buffer, 0, count, token);
        }

        public void Close()
        {
            _stream?.Close();
        }
        public void Dispose()
        {
            _stream?.Dispose();
        }

    }
}