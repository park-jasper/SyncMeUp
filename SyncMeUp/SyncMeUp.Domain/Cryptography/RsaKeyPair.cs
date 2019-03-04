using System;
using Newtonsoft.Json;

namespace SyncMeUp.Domain.Cryptography
{
    public class RsaKeyPair
    {
        public byte[] Modulus { get; }
        public RsaPrivateKey PrivateKey { get; }
        public RsaPublicKey PublicKey { get; }

        public RsaKeyPair(RsaPrivateKey privateKey, RsaPublicKey publicKey)
        {
            if (privateKey.Modulus.Length != publicKey.Modulus.Length)
            {
                throw new ArgumentException("Inequal modulus");
            }

            for (int i = 0; i < privateKey.Modulus.Length; i += 1)
            {
                if (privateKey.Modulus[i] != publicKey.Modulus[i])
                {
                    throw new ArgumentException("Inequal modulus");
                }
            }

            Modulus = privateKey.Modulus;
            PrivateKey = privateKey;
            PublicKey = publicKey;
        }

        private class SerializedKeyPair
        {
            public string ModulusBase64 { get; set; }
            public string PublicKeyExponentBase64 { get; set; }
            public string PrivateKeyExponentBase64 { get; set; }
        }

        public static string Serialize(RsaKeyPair keyPair)
        {
            var serialized = new SerializedKeyPair()
            {
                ModulusBase64 = Convert.ToBase64String(keyPair.Modulus),
                PublicKeyExponentBase64 = Convert.ToBase64String(keyPair.PublicKey.PublicKeyExponent),
                PrivateKeyExponentBase64 = Convert.ToBase64String(keyPair.PrivateKey.PrivateKeyExponent)
            };
            return JsonConvert.SerializeObject(serialized, Formatting.None);
        }

        public static RsaKeyPair Deserialize(string serializedKeyPair)
        {
            if (string.IsNullOrEmpty(serializedKeyPair))
            {
                return null;
            }
            var obj = JsonConvert.DeserializeObject<SerializedKeyPair>(serializedKeyPair);
            var modulus = Convert.FromBase64String(obj.ModulusBase64);
            return new RsaKeyPair(
                new RsaPrivateKey(modulus, Convert.FromBase64String(obj.PrivateKeyExponentBase64)),
                new RsaPublicKey(modulus, Convert.FromBase64String(obj.PublicKeyExponentBase64)));
        }
    }

    public class RsaPrivateKey
    {
        public byte[] Modulus { get; }
        public byte[] PrivateKeyExponent { get; }

        public RsaPrivateKey(byte[] modulus, byte[] privateKeyExponent)
        {
            Modulus = modulus;
            PrivateKeyExponent = privateKeyExponent;
        }
    }

    public class RsaPublicKey
    {
        public byte[] Modulus { get; }
        public byte[] PublicKeyExponent { get; }
        public RsaPublicKey(byte[] modulus, byte[] publicKeyExponent)
        {
            Modulus = modulus;
            PublicKeyExponent = publicKeyExponent;
        }
    }
}