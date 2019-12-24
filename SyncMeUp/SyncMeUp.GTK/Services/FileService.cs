using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using SyncMeUp.Domain.Contracts;

namespace SyncMeUp.GTK.Services
{
    public class FileService : IFileService
    {
        public bool ExistsFile(string path)
        {
            return File.Exists(path);
        }

        public bool ExistsDirectory(string path)
        {
            return Directory.Exists(path);
        }

        public Task<ulong> GetFileSizeInBytesAsync(string path)
        {
            var info = new FileInfo(path);
            return Task.FromResult((ulong) info.Length);
        }

        public Task<byte[]> GetFileContentsAsync(string path)
        {
            return Task.FromResult(File.ReadAllBytes(path));
        }

        public Task<IEnumerable<string>> ListDirectoriesAsync(string path)
        {
            var dirInfo = new DirectoryInfo(path);
            return Task.FromResult(dirInfo.GetDirectories().Select(f => f.Name));
        }

        public Task<IEnumerable<string>> ListFilesAsync(string path)
        {
            var dirInfo = new DirectoryInfo(path);
            return Task.FromResult(dirInfo.GetFiles().Select(f => f.Name));
        }
    }
}