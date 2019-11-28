using System;

namespace SyncMeUp.Domain.Contracts
{
    public interface IStorageEntry
    {
        string Key { get; set; }
        long SizeInBytes { get; set; }
        DateTime LastChanged { get; set; }
        Guid ChangedBy { get; set; }
    }
}