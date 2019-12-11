using System;

namespace SyncMeUp.Domain.Model
{
    public class SynchronizationFile
    {
        public string FileName { get; set; }
        public string FullRelativePath { get; set; }
        public DateTime LastChangeDate { get; set; }
        public ulong SizeInBytes { get; set; }
        public byte[] ContentHash { get; set; }
    }
}