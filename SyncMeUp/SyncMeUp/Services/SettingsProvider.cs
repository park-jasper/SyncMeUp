using System;
using SyncMeUp.Domain.Services;
using Xamarin.Essentials;

namespace SyncMeUp.Services
{
    public class SettingsProvider : ISettingsProvider
    {

        public void Store(string key, string value)
        {
            Preferences.Set(key, value);
        }
        public void Store(string key, DateTime value)
        {
            Preferences.Set(key, value);
        }

        public string Load(string key, string defaultValue)
        {
            return Preferences.Get(key, defaultValue);
        }
        public DateTime Load(string key, DateTime defaultValue)
        {
            return Preferences.Get(key, defaultValue);
        }
    }
}