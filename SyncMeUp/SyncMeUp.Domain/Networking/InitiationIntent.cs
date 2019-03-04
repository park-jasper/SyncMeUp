using System;
using SyncMeUp.Domain.Cryptography;

namespace SyncMeUp.Domain.Networking
{
    public class InitiationIntent
    {
        public InitiationMode Mode { get; }
        public Guid ServerGuid { get; private set; }
        public byte[] Otp { get; private set; }
        public RsaPrivateKey ClientPrivateKey { get; private set; }
        public RsaPublicKey ClientPublicKey { get; private set; }

        private InitiationIntent(InitiationMode mode)
        {
            Mode = mode;
        }

        public static InitiationIntent GetStandardInitiationIntent(RsaPrivateKey clientPrivateKey)
        {
            return new InitiationIntent(InitiationMode.Standard)
            {
                ClientPrivateKey = clientPrivateKey
            };
        }

        public static InitiationIntent GetOtpInitiationIntent(
            Guid serverGuid, 
            byte[] otp,
            RsaPublicKey clientPublicKey)
        {
            return new InitiationIntent(InitiationMode.Otp)
            {
                ServerGuid = serverGuid,
                Otp = otp,
                ClientPublicKey = clientPublicKey
            };
        }
    }
}