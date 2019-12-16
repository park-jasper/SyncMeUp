using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Android;
using Java.IO;
using Java.Nio.FileNio;
using SyncMeUp.Domain.Contracts;
using SyncMeUp.Domain.Services;

namespace SyncMeUp.Droid.Services
{
    public class FileService : IFileService
    {
        private Task<bool> CheckPermissions()
        {
            return Di.GetInstance<IPermissionRequestProvider>()
                .CheckAndRequestPermissionAsync(Manifest.Permission.ReadExternalStorage);
        }
        public bool ExistsFile(string path)
        {
            var file = new File(path);
            return file.Exists();
        }

        public bool ExistsDirectory(string path)
        {
            return ExistsFile(path);
        }

        public Task<ulong> GetFileSizeInBytesAsync(string path)
        {
            var file = new File(path);
            return Task.FromResult<ulong>((ulong) file.Length());

            //var file = new File(path);
            //if (file.IsFile && file.CanRead())
            //{
            //    return Task.FromResult<ulong>((ulong) file.Length());
            //}
            //else
            //{
            //    return Task.FromResult<ulong>(0);
            //}
        }

        public Task<byte[]> GetFileContentsAsync(string path)
        {
            var file = new File(path);
            return Task.FromResult(Files.ReadAllBytes(FileSystems.Default.GetPath(file.Path)));

            //var file = new File(path);
            //if (file.IsFile && file.CanRead())
            //{
            //    //var result = FileUtils.ReadFileToByteArray(file);
            //    return Task.FromResult(Files.ReadAllBytes(FileSystems.Default.GetPath(file.Path)));
            //}
            //else
            //{
            //    return Task.FromResult<byte[]>(null);
            //}
        }

        public async Task<IEnumerable<string>> ListDirectoriesAsync(string path)
        {
            var file = new File(path);
            var filter = new FileFilter(f => f.IsDirectory);
            var dirs = await file.ListFilesAsync(filter);
            return dirs.Select(d => d.Path);

            //var file = new File(path);
            //if (file.IsDirectory && file.CanRead())
            //{
            //    var filter = new FileFilter(f => f.IsDirectory);
            //    var dirs = await file.ListFilesAsync(filter);
            //    return dirs.Select(d => d.Path);
            //}
            //else
            //{
            //    return Enumerable.Empty<string>();
            //}
        }

        public async Task<IEnumerable<string>> ListFilesAsync(string path)
        {
            if (!await CheckPermissions())
            {
                return null;
            }
            var file = new File(path);
            var filter = new FileFilter(f => f.IsFile);
            var files = await file.ListFilesAsync(filter);
            var withoutFilter = await file.ListFilesAsync();
            var syncro = file.ListFiles();
            return files.Select(f => f.Path);

            //var file = new File(path);
            //if (file.IsDirectory && file.CanRead())
            //{
            //    var filter = new FileFilter(f => f.IsFile);
            //    var files = await file.ListFilesAsync(filter);
            //    return files.Select(f => f.Path);
            //}
            //else
            //{
            //    return Enumerable.Empty<string>();
            //}
        }

        private class FileFilter : Java.Lang.Object, IFileFilter
        {
            private readonly Func<File, bool> _predicate;
            public FileFilter(Func<File, bool> predicate)
            {
                _predicate = predicate;
            }

            public bool Accept(File pathname)
            {
                return _predicate(pathname);
            }
        }
    }
}