using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SyncMeUp.Domain.Contracts;
using SyncMeUp.Domain.Cryptography;
using SyncMeUp.Domain.Services;
using Xamarin.Essentials;

namespace SyncMeUp.Services
{
    public class SecureStorageProvider : ISecureStorageProvider
    {
        private readonly IUniqueIdentifierService _uniqueIdService;
        private readonly Lazy<BlowFish> _blowFish;
        private const string FallbackStoreFilename = "storage.safe";

        public SecureStorageProvider(IUniqueIdentifierService uniqueIdService)
        {
            _uniqueIdService = uniqueIdService;
            _blowFish = new Lazy<BlowFish>(CreateBlowFish);
        }

        private BlowFish CreateBlowFish()
        {
            var id = _uniqueIdService.GetDeviceUniqueId();
            var bytes = Encoding.UTF8.GetBytes(id);
            return new BlowFish(bytes);
        }

        private async Task<Dictionary<string, string>> GetStore()
        {
            var folder = Environment.GetFolderPath(Environment.SpecialFolder.Personal);

            byte[] buffer;
            using (var fileStream = File.OpenRead(Path.Combine(folder, FallbackStoreFilename)))
            {
                buffer = new byte[fileStream.Length];
                await fileStream.ReadAsync(buffer, 0, (int) fileStream.Length);
            }

            var decrypted = _blowFish.Value.Decrypt(buffer);
            var storeJson = Encoding.UTF8.GetString(decrypted);
           return JsonConvert.DeserializeObject<Dictionary<string, string>>(storeJson);
        }

        private async Task SaveStore(Dictionary<string, string> store)
        {
            var folder = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            var storeJson = JsonConvert.SerializeObject(folder);
            var encrypted = _blowFish.Value.Encrypt(Encoding.UTF8.GetBytes(storeJson));
            using (var fileStream = File.OpenWrite(Path.Combine(folder, FallbackStoreFilename)))
            {
                await fileStream.WriteAsync(encrypted, 0, encrypted.Length);
            }
        }

        private async Task<string> GetFallback(string key)
        {
            var store = await GetStore();
            return store.TryGetValue(key, out var result) ? result : null;
        }

        private async Task SetFallback(string key, string value)
        {
            var store = await GetStore();
            store[key] = value;
            await SaveStore(store);
        }

        public async Task<string> GetAsync(string key)
        {
            string result = null;
            try
            {
                result = await SecureStorage.GetAsync(key);
            }
            catch (Exception)
            {
            }

            return result ?? await GetFallback(key);
        }

        public async Task SetAsync(string key, string value)
        {
            try
            {
                await SecureStorage.SetAsync(key, value);
                return;
            }
            catch (Exception)
            {
            }

            await SetFallback(key, value);
        }
    }
}