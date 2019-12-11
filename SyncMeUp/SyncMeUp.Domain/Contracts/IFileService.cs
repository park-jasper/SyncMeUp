using System.Collections.Generic;
using System.Threading.Tasks;

namespace SyncMeUp.Domain.Contracts
{
    public interface IFileService
    {
        bool ExistsFile(string path);
        bool ExistsDirectory(string path);
        Task<ulong> GetFileSizeInBytesAsync(string path);
        Task<byte[]> GetFileContentsAsync(string path);
        Task<IEnumerable<string>> ListDirectoriesAsync(string path);
        Task<IEnumerable<string>> ListFilesAsync(string path);
    }
}