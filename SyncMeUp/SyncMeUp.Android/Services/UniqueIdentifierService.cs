using System;
using Android.Content;
using Android.Provider;
using SyncMeUp.Domain.Services;

namespace SyncMeUp.Droid.Services
{
    public class UniqueIdentifierService : IUniqueIdentifierService
    {
        private readonly Func<ContentResolver> _contentResolverFunc;
        public UniqueIdentifierService(Func<ContentResolver> contentResolverFunc)
        {
            _contentResolverFunc = contentResolverFunc;
        }
        public string GetDeviceUniqueId()
        {
            var result = Settings.Secure.GetString(_contentResolverFunc(), Settings.Secure.AndroidId);
            return result;
        }
    }
}