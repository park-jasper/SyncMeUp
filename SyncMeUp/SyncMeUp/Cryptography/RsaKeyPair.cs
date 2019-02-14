using System;

namespace SyncMeUp.Cryptography
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