namespace SyncMeUp.Domain.Contracts
{
    public interface IBulkStorageProvider
    {
        IBulkStorage OpenBulkStorage();
        void CloseBulkStorage(IBulkStorage storage);
    }
}