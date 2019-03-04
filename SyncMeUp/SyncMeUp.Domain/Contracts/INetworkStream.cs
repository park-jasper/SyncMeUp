using System.Threading;
using System.Threading.Tasks;

namespace SyncMeUp.Domain.Contracts
{
    public interface INetworkStream
    {
        Task<int> ReadAsync (byte[] buffer, int count, CancellationToken token);
        Task WriteAsync(byte[] buffer, int count, CancellationToken token);
    }
}