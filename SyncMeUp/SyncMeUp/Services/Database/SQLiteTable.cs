using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using SQLite;
using SyncMeUp.Domain.Contracts;

namespace SyncMeUp.Services.Database
{
    public class SQLiteTable : IStorageFile
    {
        private readonly SQLiteConnection _connection;
        private readonly string _tableName;

        private readonly string _createTableStatement;
        private readonly object[] _createTableParameters;

        public SQLiteTable(SQLiteConnection connection, string tableName)
        {
            _connection = connection;
            _tableName = tableName;

            var storageFileInfo = GetStorageFileInfo();

            var columnNames = new List<string> { storageFileInfo.PrimaryKeyColumn.ColumnName };
            var columnTypes = new List<string> { storageFileInfo.PrimaryKeyColumn.SqlTypeName };
            foreach (var column in storageFileInfo.FurtherColumns)
            {
                columnNames.Add(column.ColumnName);
                columnTypes.Add(column.SqlTypeName);
            }
            var columnTypesStatement = string.Join(",", columnTypes.Select(ct => $"? {ct}"));
            _createTableStatement = $"CREATE TABLE IF NOT EXISTS ? ({columnTypesStatement});";
            var parameters = new List<object> { _tableName };
            parameters.AddRange(columnNames);
            _createTableParameters = parameters.ToArray();
        }

        private void CheckValidTable()
        {
            _connection.Execute(_createTableStatement, _createTableParameters);
        }

        public IEnumerable<IStorageEntry> ReadAllEntries()
        {
            CheckValidTable();
            return _connection.Query<StorageFile>("select * from ?", _tableName).ToImmutableList();
        }

        public IStorageEntry ReadSingleEntry(string key)
        {
            return _connection.Query<StorageFile>("select * from ? where key=?", _tableName, key).FirstOrDefault();
        }


        private static StorageFileInfo GetStorageFileInfo()
        {
            var properties = typeof(StorageFile).GetProperties();
            var primaryKeyColumn = properties.First(p =>
                Attribute.IsDefined(p, typeof(PrimaryKeyAttribute)));
            var info = new StorageFileInfo
            {
                PrimaryKeyColumn = CreateColumnInfo(primaryKeyColumn),
                FurtherColumns = properties.Where(p => !Attribute.IsDefined(p, typeof(PrimaryKeyAttribute))).Select(CreateColumnInfo).ToArray()
            };
            return info;
        }

        private static ColumnInfo CreateColumnInfo(PropertyInfo propertyInfo)
        {
            var ci = new ColumnInfo
            {
                ColumnName = (string) propertyInfo.CustomAttributes
                    .First(ca => ca.AttributeType == typeof(ColumnAttribute)).NamedArguments.First().TypedValue.Value,
                ColumnType = propertyInfo.PropertyType
            };
            ci.SqlTypeName = MapTypeToSqlType(ci.ColumnType);
            return ci;
        }

        private static string MapTypeToSqlType(Type type)
        {
            if (type == typeof(string))
            {
                return "TEXT";
            }

            if (type == typeof(long))
            {
                return "INTEGER";
            }

            if (type == typeof(DateTime))
            {
                return "INTEGER";
            }

            if (type == typeof(Guid))
            {
                return "TEXT";
            }

            throw new NotImplementedException($"Type mapping to sqlite not implemented for type: '{type.Name}'");
        }
    }

    public class StorageFile : IStorageEntry
    {
        [PrimaryKey, Column("key")]
        public string Key { get; set; }

        [Column("sizeInBytes")]
        public long SizeInBytes { get; set; }

        [Column("lastChangedDate")]
        public DateTime LastChanged { get; set; }

        [Column("changedBy")]
        public Guid ChangedBy { get; set; }
    }

    public class StorageFileInfo
    {
        public ColumnInfo PrimaryKeyColumn { get; set; }
        public ColumnInfo[] FurtherColumns { get; set; }
    }

    public class ColumnInfo
    {
        public string ColumnName { get; set; }
        public Type ColumnType { get; set; }
        public string SqlTypeName { get; set; }
    }
}