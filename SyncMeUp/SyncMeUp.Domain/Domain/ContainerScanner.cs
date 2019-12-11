using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using SyncMeUp.Domain.Contracts;
using SyncMeUp.Domain.Model;

namespace SyncMeUp.Domain.Domain
{
    public class ContainerScanner
    {
        private readonly IFileService _fileService;
        private readonly HashAlgorithm _hashAlgorithm;
        public ContainerScanner(IFileService fileService, HashAlgorithm hashAlgorithm)
        {
            _fileService = fileService;
            _hashAlgorithm = hashAlgorithm;
        }

        public async Task<ChangeSet> ScanForChangesAsync(SynchronizationContainer container)
        {
            var result = new ChangeSet();
            await TraverseFolderForChanges(container.Content, container.Path, result);
            return result;
        }
        private async Task TraverseFolderForChanges(SynchronizationFolder folder, string basePath, ChangeSet result, string currentPath = "")
        {
            var fullPath = Path.Combine(basePath, currentPath);
            await ScanFilesForChanges(folder, fullPath, result);
            var modelState = folder.Folders.ToList();
            var fileSystemState = (await _fileService.ListDirectoriesAsync(fullPath)).ToList();
            var changes = await CalculateDifferences(modelState, fileSystemState, false, m => m.Name);
            foreach (var change in changes)
            {
                change.Folder = folder;
                result.Changes.Add(change);
            }

            var removedFolders = changes.Where(c => c.ChangeType == ChangeType.Deleted).Select(c => c.Name).ToList();
            foreach (var subfolder in folder.Folders.Where(f => !removedFolders.Contains(f.Name)))
            {
                await TraverseFolderForChanges(subfolder, basePath, result, Path.Combine(currentPath, subfolder.Name));
            }
            var addedFolders = changes.Where(c => c.ChangeType == ChangeType.Created).Select(c => c.Name).ToList();
            foreach (var newFolder in addedFolders)
            {
                await TraverseFolderForChanges(new SynchronizationFolder()
                {
                    Name = newFolder
                }, basePath, result, Path.Combine(currentPath, newFolder));
            }
        }

        private async Task ScanFilesForChanges(SynchronizationFolder folder, string path, ChangeSet result)
        {
            var modelState = folder.Files.ToList();
            var fileSystemState = (await _fileService.ListFilesAsync(path)).ToList();
            var changes = await CalculateDifferences(
                modelState,
                fileSystemState,
                true,
                m => m.FileName,
                m => m.SizeInBytes,
                m => m.ContentHash,
                f => _fileService.GetFileSizeInBytesAsync(Path.Combine(path, f)),
                async f => _hashAlgorithm.ComputeHash(await _fileService.GetFileContentsAsync(Path.Combine(path, f))));

            foreach (var change in changes)
            {
                change.Folder = folder;
                result.Changes.Add(change);
            }
        }

        private async Task<IReadOnlyList<ChangeRecord>> CalculateDifferences<T>(
            List<T> modelState,
            List<string> fileSystemState,
            bool isFiles,
            Func<T, string> getName,
            Func<T, ulong> getSizeInBytes = null,
            Func<T, byte[]> getHash = null,
            Func<string, Task<ulong>> getFileSizeInBytes = null,
            Func<string, Task<byte[]>> getFileHash = null)
        {
            if (isFiles)
            {
                if (getSizeInBytes == null || getHash == null || getFileSizeInBytes == null || getFileHash == null)
                {
                    throw new ArgumentException($"When working with files please supply all 'get' functions");
                }
            }
            List<ChangeRecord> result = new List<ChangeRecord>();
            modelState.Sort((left, right) => getName(left).CompareTo(getName(right)));
            fileSystemState.Sort((left, right) => left.CompareTo(right));
            using (var modelIt = modelState.GetEnumerator())
            using (var fileIt = fileSystemState.GetEnumerator())
            {
                bool modelDone = false;
                bool fileSystemDone = false;
                bool currentModelNotDone = false;

                if (!modelIt.MoveNext())
                {
                    modelDone = true;
                }
                else if (!fileIt.MoveNext())
                {
                    fileSystemDone = true;
                }
                else
                {
                    while (true)
                    {
                        if (getName(modelIt.Current) == fileIt.Current)
                        {
                            if (isFiles)
                            {
                                bool isSameSize = getSizeInBytes(modelIt.Current) == await getFileSizeInBytes(fileIt.Current);
                                bool isSameHash = false;
                                if (isSameSize)
                                {
                                    var hash = await getFileHash(fileIt.Current);
                                    isSameHash = ArrayEquals(hash, getHash(modelIt.Current));
                                }

                                if (!isSameSize || !isSameHash)
                                {
                                    result.Add(new ChangeRecord()
                                    {
                                        ChangeType = ChangeType.Edited,
                                        FileType = FileType.File,
                                        Name = fileIt.Current
                                    });
                                }
                            }
                            if (!modelIt.MoveNext())
                            {
                                modelDone = true;
                                break;
                            }
                            if (!fileIt.MoveNext())
                            {
                                fileSystemDone = true;
                                currentModelNotDone = true;
                                break;
                            }
                        }
                        else
                        {
                            if (getName(modelIt.Current).CompareTo(fileIt.Current) < 0)
                            {
                                //model name comes before file name lexicografically
                                result.Add(new ChangeRecord()
                                {
                                    ChangeType = ChangeType.Deleted,
                                    FileType = isFiles ? FileType.File : FileType.Folder,
                                    Name = getName(modelIt.Current)
                                });
                                if (!modelIt.MoveNext())
                                {
                                    modelDone = true;
                                    break;
                                }
                            }
                            else
                            {
                                result.Add(new ChangeRecord()
                                {
                                    ChangeType = ChangeType.Created,
                                    FileType = isFiles ? FileType.File : FileType.Folder,
                                    Name = fileIt.Current
                                });
                                if (!fileIt.MoveNext())
                                {
                                    fileSystemDone = true;
                                    break;
                                }
                            }
                        }
                    }
                }

                if (modelDone)
                {
                    while (fileIt.MoveNext())
                    {
                        result.Add(new ChangeRecord()
                        {
                            ChangeType = ChangeType.Created,
                            FileType = isFiles ? FileType.File : FileType.Folder,
                            Name = fileIt.Current
                        });
                    }
                }
                else if (fileSystemDone)
                {
                    if (currentModelNotDone)
                    {
                        result.Add(new ChangeRecord()
                        {
                            ChangeType = ChangeType.Deleted,
                            FileType = isFiles ? FileType.File : FileType.Folder,
                            Name = getName(modelIt.Current)
                        });
                    }
                    while (modelIt.MoveNext())
                    {
                        result.Add(new ChangeRecord()
                        {
                            ChangeType = ChangeType.Deleted,
                            FileType = isFiles ? FileType.File : FileType.Folder,
                            Name = getName(modelIt.Current)
                        });
                    }
                }
            }
            return result;
        }

        private bool ArrayEquals<TArrayType>(TArrayType[] left, TArrayType[] right) where TArrayType : IEquatable<TArrayType>
        {
            if (left.Length != right.Length)
            {
                return false;
            }
            for (int i = 0; i < left.Length; i += 1)
            {
                if (!left[i].Equals(right[i]))
                {
                    return false;
                }
            }
            return true;
        }

        public class ChangeSet
        {
            public IList<ChangeRecord> Changes { get; }

            public ChangeSet()
            {
                Changes = new List<ChangeRecord>();
            }
        }

        public class ChangeRecord
        {
            public FileType FileType { get; set; }
            public ChangeType ChangeType { get; set; }
            public string Name { get; set; }
            public SynchronizationFolder Folder { get; set; }
        }

        public enum ChangeType
        {
            Created,
            Edited,
            Deleted
        }
        public enum FileType
        {
            File,
            Folder
        }
    }
}