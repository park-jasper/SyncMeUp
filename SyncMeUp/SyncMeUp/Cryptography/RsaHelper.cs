using System;
using System.Security.Cryptography;

namespace SyncMeUp.Cryptography
{
    public static class RsaHelper
    {
        public static byte[] Encrypt(RsaPublicKey key, byte[] content)
        {
            var rsa = new RSACryptoServiceProvider();
            rsa.ImportParameters(new RSAParameters
            {
                Modulus = key.Modulus,
                Exponent = key.PublicKeyExponent
            });
            return rsa.Encrypt(content, RSAEncryptionPadding.OaepSHA512);
        }

        public static byte[] Decrypt(RsaPrivateKey key, byte[] cipher)
        {
            var rsa = new RSACryptoServiceProvider();
            rsa.ImportParameters(new RSAParameters
            {
                Modulus = key.Modulus,
                D = key.PrivateKeyExponent
            });
            return rsa.Decrypt(cipher, RSAEncryptionPadding.OaepSHA512);
            throw new NotImplementedException();
        }

        public static RsaKeyPair GenerateRsaKeyPair()
        {
            var rsa = new RSACryptoServiceProvider(2048);
            var parameters = rsa.ExportParameters(true);
            return new RsaKeyPair(
                new RsaPrivateKey(parameters.Modulus, parameters.D),
                new RsaPublicKey(parameters.Modulus, parameters.Exponent));
        }
    }
}