namespace SyncMeUp.Domain.Contracts
{
    public interface IBulkStorage
    {
        IStorageFile OpenFile(string filename);
    }
}