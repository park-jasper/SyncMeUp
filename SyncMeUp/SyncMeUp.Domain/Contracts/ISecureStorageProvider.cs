using System.Threading.Tasks;

namespace SyncMeUp.Domain.Contracts
{
    public interface ISecureStorageProvider
    {
        Task<string> GetAsync(string key);
        Task SetAsync(string key, string value);
    }
}