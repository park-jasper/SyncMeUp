using System;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using MoreLinq;
using SyncMeUp.Domain.Contracts;
using SyncMeUp.Domain.Cryptography;

namespace SyncMeUp.Domain.Networking
{
    public static class InitializationHandler
    {
        private static readonly RNGCryptoServiceProvider RandomSource = new RNGCryptoServiceProvider();
        public static async Task<NetworkResult<InitiationResult>> HandleInitializationOfClient(
            INetworkStream stream, 
            Guid serverGuid,
            Func<byte[]> getCurrentOtp, 
            Func<Guid, RsaPublicKey> getClientPublicKey,
            Action<Guid, RsaPublicKey> addClientPublicKey,
            CancellationToken token)
        {
            if (!await VerifyPresharedKey(stream, token))
            {
                return new NetworkResult<InitiationResult>{Successful = false};
            }
            await SendGuid(stream, serverGuid, token);
            var clientGuidResult = await GetGuid(stream, token);
            if (!clientGuidResult.Successful)
            {
                return new NetworkResult<InitiationResult> { Successful = false };
            }
            var initiationMode = await GetInitiationMode(stream, token);
            switch (initiationMode)
            {
                case InitiationMode.Error:
                    return new NetworkResult<InitiationResult> { Successful = false };
                case InitiationMode.Otp:
                    await HandleClientRegistrationOverOtp(stream, clientGuidResult.Result, getCurrentOtp, addClientPublicKey, token);
                    return new NetworkResult<InitiationResult>
                    {
                        Successful = true,
                        Result = new InitiationResult
                        {
                            Mode = InitiationMode.Otp
                        }
                    };
                case InitiationMode.Standard:
                    break;
                default:
                    throw new NotImplementedException($"Forgot new InitiationMode {initiationMode}");
            }
            //Veritfy guid against guid list of clients
            //Check that server has the public key
            var clientPublicKey = getClientPublicKey(clientGuidResult.Result);
            var sessionKey = GenerateSessionKey();
            await SendSessionKey(stream, clientPublicKey, sessionKey, token);
            return new NetworkResult<InitiationResult>
            {
                Successful = true,
                Result = new InitiationResult
                {
                    Mode = InitiationMode.Standard,
                    SessionKey = sessionKey
                }
            };
        }

        public static async Task<NetworkResult<object>> ConnectToServer(
            INetworkStream stream,
            Guid clientGuid,
            InitiationIntent intent,
            CancellationToken token)
        {
            var sendKeyResult = await SendPresharedKey(stream, token);
            if (!sendKeyResult.Successful)
            {
                return NetworkResult<object>.From(sendKeyResult);
            }
            var serverGuidResult = await GetGuid(stream, token);
            if (!serverGuidResult.Successful)
            {
                return NetworkResult<object>.From(serverGuidResult);
            }
            var sendGuidResult = await SendGuid(stream, clientGuid, token);
            if (!sendGuidResult.Successful)
            {
                return NetworkResult<object>.From(sendGuidResult);
            }
            var initiationModeResult = await SendInitiationMode(stream, intent.Mode, token);
            if (!initiationModeResult.Successful)
            {
                return NetworkResult<object>.From(initiationModeResult);
            }

            switch (intent.Mode)
            {
                case InitiationMode.Standard:
                    var result = await EstablishConnectionToServer(stream, clientGuid, serverGuidResult.Result,
                        intent.ClientPrivateKey, token);
                    return new NetworkResult<object> { Successful = true, Result = result };
                case InitiationMode.Otp:
                    if (serverGuidResult.Result != intent.ServerGuid)
                    {
                        return new NetworkResult<object> { Successful = false };
                    }
                    await RegisterWithServerOverOtp(stream, intent.Otp, intent.ClientPublicKey, token);
                    return new NetworkResult<object>{Successful = true};
                case InitiationMode.Error:
                default:
                    return new NetworkResult<object> { Successful = false };
            }
        }

        private static async Task HandleClientRegistrationOverOtp(
            INetworkStream stream, 
            Guid clientGuid,
            Func<byte[]> getCurrentOtp,
            Action<Guid, RsaPublicKey> addClientPublicKey,
            CancellationToken token)
        {
            byte[] buffer = new byte[4];
            int length = await stream.ReadAsync(buffer, buffer.Length, token);
            if (length != buffer.Length)
            {
                return;
            }

            var dataType = ConvertToCommunicationData(buffer);
            if (dataType != CommunicationData.PublicKey)
            {
                return;
            }

            byte[] lengthInfo = new byte[9];
            length = await stream.ReadAsync(lengthInfo, lengthInfo.Length, token);
            if (length != lengthInfo.Length)
            {
                return;
            }

            var encryptedCombinationLength = BitConverter.ToInt32(lengthInfo, 0);
            var decryptedModulusLength = BitConverter.ToInt32(lengthInfo, 4);
            var exponentPadding = lengthInfo[8];

            byte[] encryptedCombination = new byte[encryptedCombinationLength];
            length = await stream.ReadAsync(encryptedCombination, encryptedCombinationLength, token);
            if (length != encryptedCombinationLength)
            {
                return;
            }

            var blowFish = new BlowFish(getCurrentOtp());
            var decryptedCombination = blowFish.Decrypt(encryptedCombination);
            var modulus = new byte[decryptedModulusLength];
            var decryptedExponentLength = decryptedCombination.Length - decryptedModulusLength - exponentPadding;
            var exponent = new byte[decryptedExponentLength];
            Array.Copy(decryptedCombination, 0, modulus, 0, decryptedModulusLength);
            Array.Copy(decryptedCombination, decryptedModulusLength, exponent, 0, decryptedExponentLength);
            var clientPublicKey = new RsaPublicKey(modulus, exponent);
            addClientPublicKey(clientGuid, clientPublicKey);
        }

        private static async Task RegisterWithServerOverOtp(
            INetworkStream stream, 
            byte[] serverOtp,
            RsaPublicKey clientPublicKey, 
            CancellationToken token)
        {
            var blowFish = new BlowFish(serverOtp);
            var combination = new byte[clientPublicKey.Modulus.Length + clientPublicKey.PublicKeyExponent.Length];
            Array.Copy(clientPublicKey.Modulus, combination, clientPublicKey.Modulus.Length);
            Array.Copy(clientPublicKey.PublicKeyExponent, 0, combination, clientPublicKey.Modulus.Length,
                clientPublicKey.PublicKeyExponent.Length);
            var encryptedCombination = blowFish.Encrypt(combination);
            await stream.WriteAsync(CommunicationData.PublicKey.ToByteArray(), 4, token);

            //4 byte encrypted combination length (int), 4 byte modulus length (int), 1 byte public exponent padding (byte)
            byte[] lengthInfo = new byte[9];
            var encryptedCombinationLength = BitConverter.GetBytes(encryptedCombination.Length);
            var decryptedModulusLength = BitConverter.GetBytes(clientPublicKey.Modulus.Length);
            var exponentPadding = (8 - combination.Length % 8) % 8;
            Array.Copy(encryptedCombinationLength, lengthInfo, 4);
            Array.Copy(decryptedModulusLength, 0, lengthInfo, 4, 4);
            lengthInfo[8] = (byte) exponentPadding;

            await stream.WriteAsync(lengthInfo, lengthInfo.Length, token);
            await stream.WriteAsync(encryptedCombination, encryptedCombination.Length, token);
        }

        private static async Task<object> EstablishConnectionToServer(
            INetworkStream stream,
            Guid clientGuid,
            Guid serverGuid,
            RsaPrivateKey clientPrivateKey,
            CancellationToken token)
        {
            //Verify guid against guid list of servers
            //Check that server has the public key
            await SendGuid(stream, clientGuid, token);
            var sessionKey = await GetSessionKey(stream, clientPrivateKey, token);
            if (sessionKey.Successful)
            {
                return new SessionControl(stream, sessionKey.Result);
            }
            else
            {
                return null; //TODO correct error result
            }
        }

        private static async Task<NetworkResult> SendPresharedKey(INetworkStream stream, CancellationToken token)
        {
            try
            {
                await stream.WriteAsync(Networking.PresharedKey, Networking.PresharedKey.Length, token);
                return new NetworkResult
                {
                    Successful = true
                };
            }
            catch (Exception exc)
            {
                return new NetworkResult
                {
                    Successful = false, Exception = exc
                };
            }
        }
        private static async Task<bool> VerifyPresharedKey(INetworkStream stream, CancellationToken token)
        {
            byte[] buffer = new byte[Networking.PresharedKey.Length];
            int messageLength = await stream.ReadAsync(buffer, buffer.Length, token);
            return messageLength == Networking.PresharedKey.Length
                   && buffer
                       .EquiZip(Networking.PresharedKey, (left, right) => left == right)
                       .All(eq => eq);
        }

        private static async Task<NetworkResult> SendInitiationMode(INetworkStream stream, InitiationMode mode, CancellationToken token)
        {
            try
            {
                await stream.WriteAsync(mode.ToByteArray(), 4, token);
                return new NetworkResult { Successful = true };
            }
            catch (Exception exc)
            {
                return new NetworkResult
                {
                    Successful = false,
                    Exception = exc
                };
            }
        }
        private static async Task<InitiationMode> GetInitiationMode(INetworkStream stream, CancellationToken token)
        {
            byte[] buffer = new byte[4];
            int messageLength = await stream.ReadAsync(buffer, buffer.Length, token);
            if (messageLength != 4)
            {
                return InitiationMode.Error;
            }

            return ConvertToInitiationMode(buffer);
        }



        private static byte[] GenerateSessionKey()
        {
            byte[] key = new byte[BlowFish.MaxKeyLength];
            RandomSource.GetBytes(key);
            return key;
        }
        private static async Task<NetworkResult<byte[]>> GetSessionKey(INetworkStream stream, RsaPrivateKey clientPrivateKey, CancellationToken token)
        {
            var buffer = new byte[4];
            var length = await stream.ReadAsync(buffer, buffer.Length, token);
            if (length != buffer.Length)
            {
                return new NetworkResult<byte[]> { Successful = false };
            }

            var dataType = ConvertToCommunicationData(buffer);
            if (dataType != CommunicationData.SessionKey)
            {
                return new NetworkResult<byte[]> { Successful = false };
            }

            length = await stream.ReadAsync(buffer, buffer.Length, token);
            if (length != 4)
            {
                return new NetworkResult<byte[]> { Successful = false };
            }

            var encryptedSessionKeySize = BitConverter.ToInt32(buffer, 0);
            var encryptedSessionKey = new byte[encryptedSessionKeySize];
            length = await stream.ReadAsync(encryptedSessionKey, encryptedSessionKeySize, token);
            if (length != encryptedSessionKeySize)
            {
                return new NetworkResult<byte[]> { Successful = false };
            }

            var decryptedSessionKey = RsaHelper.Decrypt(clientPrivateKey, encryptedSessionKey);
            return new NetworkResult<byte[]> { Successful = true, Result = decryptedSessionKey };
        }

        private static async Task SendSessionKey(INetworkStream stream, RsaPublicKey clientPublicKey, byte[] sessionKey, CancellationToken token)
        {
            var encryptedSessionKey = RsaHelper.Encrypt(clientPublicKey, sessionKey);
            await stream.WriteAsync(CommunicationData.SessionKey.ToByteArray(), 4, token);
            await stream.WriteAsync(BitConverter.GetBytes(encryptedSessionKey.Length), 4, token);
            await stream.WriteAsync(encryptedSessionKey, encryptedSessionKey.Length, token);
        }



        private static async Task<NetworkResult> SendGuid(INetworkStream stream, Guid guid, CancellationToken token)
        {
            var guidByteArray = guid.ToByteArray();
            try
            {
                await stream.WriteAsync(guidByteArray, guidByteArray.Length, token);
                return new NetworkResult { Successful = true };
            }
            catch (Exception exception)
            {
                return new NetworkResult
                {
                    Successful = false,
                    Exception = exception
                };
            }
        }

        private static async Task<NetworkResult<Guid>> GetGuid(INetworkStream stream, CancellationToken token)
        {
            var guidLength = new Guid().ToByteArray().Length;
            byte[] buffer = new byte[guidLength];
            try
            {
                int messageLength = await stream.ReadAsync(buffer, guidLength, token);
                if (messageLength == guidLength)
                {
                    return new NetworkResult<Guid> { Successful = true, Result = new Guid(buffer) };
                }
                else
                {
                    return new NetworkResult<Guid> { Successful = false };
                }
            }
            catch (Exception exc)
            {
                return new NetworkResult<Guid>
                {
                    Successful = false,
                    Exception = exc
                };
            }
        }



        private static InitiationMode ConvertToInitiationMode(byte[] bytes)
        {
            if (bytes.Length != 4)
            {
                return InitiationMode.Error;
            }
            var value = BitConverter.ToInt32(bytes, 0);
            return (InitiationMode)value;
        }
        private static CommunicationData ConvertToCommunicationData(byte[] bytes)
        {
            if (bytes.Length != 4)
            {
                return CommunicationData.Error;
            }
            var value = BitConverter.ToInt32(bytes, 0);
            return (CommunicationData)value;
        }
    }
}