using System;

namespace SyncMeUp.Domain.Services
{
    public interface ISettingsProvider
    {
        void Store(string key, string value);
        void Store(string key, DateTime value);

        string Load(string key, string defaultValue);
        DateTime Load(string key, DateTime defaultValue);
    }
}