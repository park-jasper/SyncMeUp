using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using SyncMeUp.Domain.Contracts;

namespace SyncMeUp.Test.Mocking
{
    public class FileServiceMock : IFileService
    {
        private readonly TestFolder _base;

        public FileServiceMock(string basePath)
        {
            _base = new TestFolder()
            {
                Name = basePath,
                Files = new List<TestFile>(),
                Folders = new List<TestFolder>()
            };
        }
        public bool ExistsFile(string path)
        {
            var (folder, file) = FindFileFolder(path);
            return folder?.Files.Any(f => f.Name == file) ?? false;
        }

        public bool ExistsDirectory(string path)
        {
            var folder = FindFolder(path);
            return folder?.Folders.Any(f => f.Name == path) ?? false;
        }

        public Task<ulong> GetFileSizeInBytesAsync(string path)
        {
            var (folder, file) = FindFileFolder(path);
            return Task.FromResult(folder?.Files.FirstOrDefault(f => f.Name == file)?.SizeInBytes ?? 0);
        }

        public Task<byte[]> GetFileContentsAsync(string path)
        {
            var (folder, file) = FindFileFolder(path);
            return Task.FromResult(folder?.Files.FirstOrDefault(f => f.Name == file)?.Content);
        }

        public Task<IEnumerable<string>> ListDirectoriesAsync(string path)
        {
            return Task.FromResult(FindFolder(path)?.Folders.Select(f => f.Name));
        }

        public Task<IEnumerable<string>> ListFilesAsync(string path)
        {
            return Task.FromResult(FindFolder(path)?.Files.Select(f => f.Name));
        }

        private TestFolder FindFolder(string path)
        {
            var parts = path.Split(Path.DirectorySeparatorChar);
            return FindFolder(parts.Skip(1));
        }
        private (TestFolder, string) FindFileFolder(string path)
        {
            var array = path.Split(Path.DirectorySeparatorChar);
            var parts = array.Skip(1).Take(array.Length - 2);
            return (FindFolder(parts), array[array.Length - 1]);
        }
        private TestFolder FindFolder(IEnumerable<string> parts)
        {
            TestFolder folder = _base;
            foreach (var part in parts)
            {
                folder = folder?.Folders.FirstOrDefault(f => f.Name == part);
            }
            return folder;
        }

        public void AddFiles(IEnumerable<TestFile> files)
        {
            _base.Files.AddRange(files);
        }
        public void AddFolders(IEnumerable<TestFolder> folders)
        {
            _base.Folders.AddRange(folders);
        }

        public TestFile File(string name, ulong sizeInBytes, byte content1, byte content2 = 0, byte content3 = 0, byte content4 = 0)
        {
            return new TestFile()
            {
                Name = name,
                SizeInBytes = sizeInBytes,
                Content = new[] { content1, content2, content3, content4 }
            };
        }
        public TestFolder Folder(string name, IEnumerable<TestFile> files, IEnumerable<TestFolder> folders = null)
        {
            return new TestFolder
            {
                Name = name,
                Files = files?.ToList() ?? new List<TestFile>(),
                Folders = folders?.ToList() ?? new List<TestFolder>()
            };
        }

        public class TestFile
        {
            public string Name { get; set; }
            public ulong SizeInBytes { get; set; }
            public byte[] Content { get; set; }
        }

        public class TestFolder
        {
            public string Name { get; set; }
            public List<TestFile> Files { get; set; }
            public List<TestFolder> Folders { get; set; }
        }
    }
}