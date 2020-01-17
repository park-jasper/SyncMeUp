using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Windows.Security.Credentials;
using Windows.Security.ExchangeActiveSyncProvisioning;
using Windows.System;
using SyncMeUp.Domain.Services;

namespace SyncMeUp.UWP.Services
{
    public class UniqueIdentifierService : IUniqueIdentifierService
    {
        public const string UniqueIdName = "unique_device_id";
        public const string SyncMeUpUsername = "SyneMeUp";
        public string GetDeviceUniqueId()
        {
            var vault = new PasswordVault();
            var list = vault.FindAllByResource(UniqueIdName);
            if (list.Count > 0)
            {
                return list[0].Password;
            }

            var randomSource = new RNGCryptoServiceProvider();
            var buffer = new byte[40];
            randomSource.GetBytes(buffer);
            var idString = Convert.ToBase64String(buffer);
            vault.Add(new PasswordCredential(UniqueIdName, SyncMeUpUsername, idString));
            return idString;
        }
    }
}