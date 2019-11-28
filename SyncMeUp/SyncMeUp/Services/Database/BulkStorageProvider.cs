using SyncMeUp.Domain.Contracts;

namespace SyncMeUp.Services.Database
{
    public class BulkStorageProvider : IBulkStorageProvider
    {
        private readonly string _databaseFile;
        private readonly object _databaseConnectionHandle = new object();

        private SQLiteDatabase _database;
        private int _databaseOpenCount = 0;

        public BulkStorageProvider(string databaseFile)
        {
            _databaseFile = databaseFile;
        }

        public IBulkStorage OpenBulkStorage()
        {
            lock (_databaseConnectionHandle)
            {
                _databaseOpenCount += 1;
                if (_database != null)
                {
                    return _database;
                }
                else
                {
                    return OpenDatabase();
                }
            }
        }

        public void CloseBulkStorage(IBulkStorage storage)
        {
            lock (_databaseConnectionHandle)
            {
                _databaseOpenCount -= 1;
                if (_databaseOpenCount == 0)
                {
                    CloseDatabase(_database);
                    _database = null;
                }
            }
        }

        private SQLiteDatabase OpenDatabase()
        {
            return new SQLiteDatabase(_databaseFile);
        }

        private static void CloseDatabase(SQLiteDatabase database)
        {
            database.Dispose();
        }
    }
}