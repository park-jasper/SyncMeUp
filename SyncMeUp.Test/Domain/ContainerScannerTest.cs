using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using SyncMeUp.Domain.Domain;
using SyncMeUp.Domain.Model;
using SyncMeUp.Test.Contracts;
using SyncMeUp.Test.Mocking;
using static SyncMeUp.Domain.Domain.ContainerScanner;

namespace SyncMeUp.Test.Domain
{
    public class ContainerScannerTest : TestBase
    {
        public ContainerScannerTest(IAssert assert) : base(assert)
        {
        }

        private class TestFile
        {
            public string Name { get; set; }
            public ulong Size { get; set; }
            public byte[] Hash { get; set; }
        }

        public async Task TestFileScanning()
        {
            var models = new List<TestFile>
            {
                new TestFile
                {
                    Name = "unchanged",
                    Size = 1234,
                    Hash = new byte[]{1,2,3,4}
                },
                new TestFile
                {
                    Name = "deleted",
                    Size = 2345,
                    Hash = new byte[]{2,3,4, 5}
                },
                new TestFile
                {
                    Name = "content-changed",
                    Size = 3456,
                    Hash = new byte[]{3,4,5,6}
                },
                new TestFile{
                    Name = "size-changed",
                    Size = 4567,
                    Hash = new byte[]{4,5,6,7}
                }
            };
            var files = new List<string>
            {
                "unchanged",
                "created",
                "content-changed",
                "size-changed"
            };
            var sizeDict = new Dictionary<string, ulong>
            {
                { "unchanged", 1234 },
                { "created", 4321 },
                { "content-changed", 3456 },
                { "size-changed", 1 }
            };
            var hashDict = new Dictionary<string, byte[]>
            {
                { "unchanged", new byte[] { 1, 2, 3, 4 } },
                { "created", new byte[] { 1 } },
                { "content-changed", new byte[] { 1 } },
                { "size-changed", new byte[] { 4, 3, 2, 1 } }
            };
            var fileServiceMock = new FileServiceMock("");
            var scanner = new ContainerScanner(fileServiceMock, SHA512.Create());
            var result = await scanner.CalculateDifferences(models, files, true, m => m.Name, m => m.Size, m => m.Hash,
                f => Task.FromResult(sizeDict[f]), f => Task.FromResult(hashDict[f]));

            Assert.IsFalse(result.Any(c => c.Name == "unchanged"), "unchanged file was added even though it did not change");

            var deletedChange = result.FirstOrDefault(c => c.Name == "deleted");
            Assert.IsNotNull(deletedChange, "deleted file not found");
            Assert.AreEqual(ChangeType.Deleted, deletedChange.ChangeType, "deleted file wrong change type");

            var createdChange = result.FirstOrDefault(c => c.Name == "created");
            Assert.IsNotNull(createdChange, "created file not found");
            Assert.AreEqual(ChangeType.Created, createdChange.ChangeType,
                "created file wrong change type");

            var contentChange = result.FirstOrDefault(c => c.Name == "content-changed");
            Assert.IsNotNull(contentChange, "content-changed not found");
            Assert.AreEqual(ChangeType.Edited, contentChange.ChangeType, "content-changed wrong change type");

            var sizeChange = result.FirstOrDefault(c => c.Name == "size-changed");
            Assert.IsNotNull(sizeChange, "size-changed not found");
            Assert.AreEqual(ChangeType.Edited, sizeChange.ChangeType);

            Assert.AreEqual(result.Count, 4, "recognized too many changes");

            Assert.IsTrue(result.All(r => r.FileType == FileType.File), "changing files marked as folder");
        }

        public async Task TestNestedFolderScanAllNewFiles()
        {
            var models = new List<string>();
            var fs = new FileServiceMock("relative-folder");
            //  relative-folder
            //      Documents
            //          lyrics.txt
            //          slides.pdf
            //      Music
            //          snoop-dog.mp3
            //          RHCP
            //              snow.mp3
            //      Videos
            //      University
            //          Exams
            //              programming.doc
            //      puppies.mp4
            //      cartman.mkv
            fs.AddFolders(new[]
            {
                fs.Folder("Documents", new[]
                {
                    fs.File("lyrics.txt", 1, 1),
                    fs.File("slides.pdf", 2, 2),
                }),
                fs.Folder("Music", new[]
                {
                    fs.File("snoop-dog.mp3", 3, 3),
                }, new[]
                {
                    fs.Folder("RHCP", new[]
                    {
                        fs.File("snow.mp3", 4, 4)
                    })
                }),
                fs.Folder("Videos", null),
                fs.Folder("University", null, new[]
                {
                    fs.Folder("Exams", new[]
                    {
                        fs.File("programming.doc", 5, 5)
                    })
                })
            });
            fs.AddFiles(new[]
            {
                fs.File("puppies.mp4", 6, 6),
                fs.File("cartman.mkv", 7, 7)
            });

            var scanner = new ContainerScanner(fs, SHA512.Create());
            var container = new SynchronizationContainer()
            {
                Path = "relative-folder",
                Content = new SynchronizationFolder()
                {
                    Name = "my-relative-folder"
                }
            };
            var result = await scanner.ScanForChangesAsync(container);
            var changes = result.Changes;

            Assert.IsTrue(changes.All(c => c.ChangeType == ChangeType.Created));
            Assert.AreEqual(13, changes.Count, "wrong amount of changes");
            Assert.AreEqual(6, changes.Count(c => c.FileType == FileType.Folder), "wrong number of new folders");
            Assert.AreEqual(7, changes.Count(c => c.FileType == FileType.File), "wrong number of new files");

            CheckParentFolder(changes, "lyrics.txt", "Documents");
            CheckParentFolder(changes, "snow.mp3", "RHCP");
            CheckParentFolder(changes, "RHCP", "Music");
            CheckParentFolder(changes, "programming.doc", "Exams");
            CheckParentFolder(changes, "Exams", "University");
        }

        private void CheckParentFolder(IEnumerable<ChangeRecord> changes, string fileOrFolderName, string parentFolderName)
        {
            Assert.AreEqual(parentFolderName, changes.FirstOrDefault(c => c.Name == fileOrFolderName)?.ParentFolder.Name,
                $"{fileOrFolderName} in wrong folder");
        }
    }
}