using System;
using SyncMeUp.Domain.Cryptography;

namespace SyncMeUp.Domain.Networking
{
    public class SyncMeUpInstance
    {
        public string Name { get; set; }
        public Guid Guid { get; set; }
        public RsaPublicKey InstancePublicKey { get; set; }
    }
}