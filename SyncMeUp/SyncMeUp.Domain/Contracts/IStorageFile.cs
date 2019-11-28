using System.Collections.Generic;

namespace SyncMeUp.Domain.Contracts
{
    public interface IStorageFile
    {
        IEnumerable<IStorageEntry> ReadAllEntries();
        IStorageEntry ReadSingleEntry(string key);
    }
}