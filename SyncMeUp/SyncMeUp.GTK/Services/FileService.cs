using System.Collections.Generic;
using System.Threading.Tasks;
using SyncMeUp.Domain.Contracts;

namespace SyncMeUp.GTK.Services
{
    public class FileService : IFileService
    {
        public bool ExistsFile(string path)
        {
            throw new System.NotImplementedException();
        }

        public bool ExistsDirectory(string path)
        {
            throw new System.NotImplementedException();
        }

        public Task<ulong> GetFileSizeInBytesAsync(string path)
        {
            throw new System.NotImplementedException();
        }

        public Task<byte[]> GetFileContentsAsync(string path)
        {
            throw new System.NotImplementedException();
        }

        public Task<IEnumerable<string>> ListDirectoriesAsync(string path)
        {
            throw new System.NotImplementedException();
        }

        public Task<IEnumerable<string>> ListFilesAsync(string path)
        {
            throw new System.NotImplementedException();
        }
    }
}