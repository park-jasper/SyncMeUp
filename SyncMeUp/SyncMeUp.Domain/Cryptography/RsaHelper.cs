using System;
using System.Security.Cryptography;
using System.Threading.Tasks;
using SyncMeUp.Domain.Contracts;
using SyncMeUp.Domain.Services;

namespace SyncMeUp.Domain.Cryptography
{
    public static class RsaHelper
    {
        public const string RsaDeviceKeyPairIdentifier = "device_rsa_key_pair";
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
#if DEBUG
            return RsaTestHelper.TestKeyPair;
#else
            var rsa = new RSACryptoServiceProvider(2048);
            var parameters = rsa.ExportParameters(true);
            return new RsaKeyPair(
                new RsaPrivateKey(parameters.Modulus, parameters.D),
                new RsaPublicKey(parameters.Modulus, parameters.Exponent));
#endif
        }

        public static async Task<RsaKeyPair> GetStoredRsaKey()
        {
            try
            {
                var key = await Di.GetInstance<ISecureStorageProvider>().GetAsync(RsaDeviceKeyPairIdentifier);
                return RsaKeyPair.Deserialize(key);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static async Task StoreRsaKey(RsaKeyPair keyPair)
        {
            try
            {
                await Di.GetInstance<ISecureStorageProvider>().SetAsync(RsaDeviceKeyPairIdentifier, RsaKeyPair.Serialize(keyPair));
            }
            catch (Exception)
            {

            }
        }
    }
#if DEBUG
    public static class RsaTestHelper
    {
        private static readonly byte[] TestModulus = new byte[]
        {
            199, 104, 43, 84, 137, 59, 239, 109, 168, 82, 4, 22, 233, 8, 178, 212, 1, 138, 112, 24, 11, 66, 187, 126,
            162, 86, 147, 216, 80, 235, 187, 253, 17, 108, 28, 178, 109, 156, 55, 134, 4, 168, 119, 33, 76, 13, 185,
            184, 1, 69, 236, 42, 31, 138, 221, 175, 215, 137, 108, 198, 16, 197, 169, 255, 101, 75, 182, 233, 11, 172,
            214, 54, 164, 155, 39, 43, 61, 8, 10, 59, 169, 156, 17, 180, 44, 133, 177, 173, 247, 154, 194, 128, 159, 25,
            224, 181, 71, 123, 101, 232, 255, 10, 234, 46, 77, 54, 96, 201, 138, 116, 174, 136, 23, 171, 73, 38, 87, 28,
            85, 124, 191, 114, 179, 3, 130, 75, 121, 35, 20, 196, 50, 146, 161, 120, 65, 239, 187, 17, 178, 249, 222,
            21, 103, 104, 137, 81, 130, 84, 39, 122, 228, 133, 95, 226, 33, 92, 13, 28, 27, 220, 33, 102, 172, 13, 68,
            239, 204, 116, 221, 56, 74, 194, 148, 88, 73, 40, 60, 138, 185, 123, 31, 118, 109, 252, 134, 185, 211, 142,
            48, 240, 128, 239, 49, 58, 145, 202, 189, 14, 183, 149, 71, 59, 188, 144, 66, 155, 114, 220, 247, 186, 54,
            51, 208, 165, 240, 162, 150, 105, 143, 163, 128, 124, 178, 170, 86, 96, 245, 101, 128, 190, 118, 10, 161,
            158, 98, 76, 79, 168, 94, 31, 13, 47, 65, 145, 204, 229, 224, 55, 54, 3, 52, 138, 6, 45, 28, 65
        };

        private static readonly byte[] TestPrivateExponent = new byte[]
        {
            75, 40, 34, 244, 184, 200, 195, 68, 68, 101, 173, 46, 233, 100, 34, 234, 175, 45, 251, 115, 196, 130, 193,
            98, 72, 83, 115, 99, 219, 148, 14, 5, 163, 20, 105, 120, 130, 193, 151, 87, 198, 215, 172, 22, 251, 176, 76,
            168, 98, 170, 117, 9, 167, 91, 210, 148, 93, 27, 105, 200, 249, 55, 87, 12, 112, 164, 105, 235, 74, 64, 57,
            120, 220, 239, 177, 130, 165, 125, 43, 70, 51, 118, 36, 98, 17, 73, 206, 159, 48, 44, 191, 84, 117, 34, 238,
            195, 196, 142, 104, 233, 185, 126, 230, 166, 206, 233, 72, 239, 141, 100, 207, 7, 114, 76, 87, 196, 192,
            158, 104, 70, 29, 110, 96, 180, 88, 222, 147, 104, 14, 223, 250, 119, 24, 161, 7, 196, 74, 15, 13, 245, 11,
            11, 44, 252, 234, 165, 24, 162, 216, 215, 216, 55, 218, 97, 178, 194, 96, 226, 21, 208, 94, 186, 8, 24, 102,
            218, 209, 91, 182, 133, 248, 250, 211, 70, 125, 177, 5, 84, 93, 98, 138, 242, 169, 206, 4, 238, 79, 56, 55,
            53, 38, 20, 22, 233, 105, 185, 224, 250, 134, 188, 74, 130, 7, 69, 68, 218, 15, 129, 255, 3, 42, 77, 59, 38,
            165, 57, 68, 60, 208, 155, 241, 220, 189, 255, 94, 102, 222, 193, 209, 202, 246, 105, 194, 61, 238, 36, 44,
            52, 27, 175, 144, 168, 195, 158, 202, 166, 3, 116, 97, 7, 57, 166, 221, 6, 7, 181
        };

        private static readonly byte[] TestPublicExponent = new byte[] { 1, 0, 1 };

        public static RsaKeyPair TestKeyPair = new RsaKeyPair(
            new RsaPrivateKey(TestModulus, TestPrivateExponent),
            new RsaPublicKey(TestModulus, TestPublicExponent));
    }
#endif
}