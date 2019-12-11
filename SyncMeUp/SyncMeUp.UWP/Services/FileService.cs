using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;
using SyncMeUp.Domain.Contracts;

namespace SyncMeUp.UWP.Services
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

        public async Task<ulong> GetFileSizeInBytesAsync(string path)
        {
            var file = await StorageFile.GetFileFromPathAsync(path);
            var properties = await file.GetBasicPropertiesAsync();
            return properties.Size;
        }

        public async Task<byte[]> GetFileContentsAsync(string path)
        {
            var file = await StorageFile.GetFileFromPathAsync(path);
            var buffer = await FileIO.ReadBufferAsync(file);
            using (var dataReader = DataReader.FromBuffer(buffer))
            {
                var result = new byte[buffer.Length];
                dataReader.ReadBytes(result);
                return result;
            }
        }

        public async Task<IEnumerable<string>> ListDirectoriesAsync(string path)
        {
            var folder = await StorageFolder.GetFolderFromPathAsync(path);
            var folders = await folder.GetFoldersAsync();
            return folders.Select(f => f.Name);
        }

        public async Task<IEnumerable<string>> ListFilesAsync(string path)
        {
            var folder = await StorageFolder.GetFolderFromPathAsync(path);
            var files = await folder.GetFilesAsync();
            return files.Select(f => f.Name);
        }
    }
}