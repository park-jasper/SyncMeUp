using System;
using SQLite;
using SyncMeUp.Domain.Contracts;

namespace SyncMeUp.Services.Database
{
    public class SQLiteDatabase : IBulkStorage, IDisposable
    {
        private SQLiteConnection _connection;
        public SQLiteDatabase(string filename)
        {
            _connection = new SQLiteConnection(filename, SQLiteOpenFlags.ReadWrite);
        }

        public IStorageFile OpenFile(string filename)
        {
            return new SQLiteTable(_connection, filename);
        }

        public void Dispose()
        {
            _connection?.Close();
            _connection?.Dispose();
            _connection = null;
        }
    }
}