using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using SyncMeUp.Cryptography;

namespace SyncMeUp.Networking
{
    public class Client : CommunicationBase
    {
        public Client(Guid clientGuid) : base(clientGuid)
        {
        }

        public ClientControl<bool> RegisterWithServer(IPAddress serverIpAddress, int port, Guid serverGuid, byte[] serverOtp)
        {
            var client = new TcpClient(Networking.AddressFamilyToUse);
            var tokenSource = new CancellationTokenSource();
            var registrationTask = Task.Run(() => RegisterWithServerInternal(client, serverIpAddress, port, serverGuid, serverOtp, tokenSource.Token));
            return new ClientControl<bool>(tokenSource, client, registrationTask);
        }

        public ClientControl<object> ConnectToServer(IPAddress serverIpAddress, int port)
        {
            var client = new TcpClient(Networking.AddressFamilyToUse);
            var tokenSource = new CancellationTokenSource();
            var connectionTask =
                Task.Run(() => ConnectToServerInternal(client, serverIpAddress, port, tokenSource.Token));
            return new ClientControl<object>(tokenSource, client, connectionTask);
        }

        private async Task<object> ConnectToServerInternal(TcpClient client, IPAddress serverIpAddress, int port,
            CancellationToken token)
        {
            await client.ConnectAsync(serverIpAddress, port);
            var stream = client.GetStream();
            await SendPresharedKey(stream, token);
            var serverGuidResult = await GetGuid(stream, token);
            if (!serverGuidResult.Successful) return null;
            //Verify guid against guid list of servers
            //Check that server has the public key
            await SendGuid(stream, token);
            RsaPrivateKey privateKey = null;
            var sessionKey = await GetSessionKey(stream, privateKey, token);

            throw new NotImplementedException();
        }

        private async Task<bool> RegisterWithServerInternal(TcpClient client, IPAddress serverIpAddress, int port, Guid serverGuid, byte[] serverOtp, CancellationToken token)
        {
            await client.ConnectAsync(serverIpAddress, port);
            using (var stream = client.GetStream())
            {
                await SendPresharedKey(stream, token);
                var serverGuidResult = await GetGuid(stream, token);
                if (!serverGuidResult.Successful)
                {
                    return false;
                }

                if (serverGuidResult.Result != serverGuid)
                {
                    return false;
                }

                RsaPublicKey publicKey = null;
                var blowFish = new BlowFish(serverOtp);
                var encryptedModulus = blowFish.Encrypt_CBC(publicKey.Modulus);
                var encryptedPublicExponent = blowFish.Encrypt_CBC(publicKey.PublicKeyExponent);
                await stream.WriteAsync(CommunicationData.PublicKey.ToByteArray(), 0, 4, token);
                byte[] keyLengths = new byte[8];
                var modulusLength = BitConverter.GetBytes(encryptedModulus.Length);
                var exponentLength = BitConverter.GetBytes(encryptedPublicExponent.Length);
                Array.Copy(modulusLength, keyLengths, 4);
                Array.Copy(exponentLength, 0, keyLengths, 4, 4);
                await stream.WriteAsync(keyLengths, 0, keyLengths.Length, token);
                await stream.WriteAsync(encryptedModulus, 0, encryptedModulus.Length, token);
                await stream.WriteAsync(encryptedPublicExponent, 0, encryptedPublicExponent.Length, token);
            }

            client.Close();
            return true;
        }

        private static async Task SendPresharedKey(NetworkStream stream, CancellationToken token)
        {
            await stream.WriteAsync(Networking.PresharedKey, 0, Networking.PresharedKey.Length, token);
        }

        private async Task<NetworkResult<byte[]>> GetSessionKey(NetworkStream stream, RsaPrivateKey clientPrivateKey, CancellationToken token)
        {
            var buffer = new byte[4];
            var length = await stream.ReadAsync(buffer, 0, buffer.Length, token);
            if (length != buffer.Length)
            {
                return new NetworkResult<byte[]> { Successful = false };
            }

            var dataType = ConvertToCommunicationData(buffer);
            if (dataType != CommunicationData.SessionKey)
            {
                return new NetworkResult<byte[]> { Successful = false };
            }

            length = await stream.ReadAsync(buffer, 0, buffer.Length, token);
            if (length != 4)
            {
                return new NetworkResult<byte[]> { Successful = false };
            }

            var encryptedSessionKeySize = BitConverter.ToInt32(buffer, 0);
            var encryptedSessionKey = new byte[encryptedSessionKeySize];
            length = await stream.ReadAsync(encryptedSessionKey, 0, encryptedSessionKeySize, token);
            if (length != encryptedSessionKeySize)
            {
                return new NetworkResult<byte[]> { Successful = false };
            }

            var decryptedSessionKey = RsaHelper.Decrypt(clientPrivateKey, encryptedSessionKey);
            return new NetworkResult<byte[]> { Successful = true, Result = decryptedSessionKey };
        }

        public class ClientControl<TTaskResult> : CommunicationsControl
        {
            public Task<TTaskResult> ConnectionTask { get; }
            private readonly TcpClient _client;

            public ClientControl(CancellationTokenSource tokenSource, TcpClient client, Task<TTaskResult> connectionTask) :
                base(tokenSource)
            {
                ConnectionTask = connectionTask;
                _client = client;
            }

            public override void Stop()
            {
                base.Stop();
                Task.Run(async () =>
                {
                    await Task.Delay(30);
                    _client.Close();
                });
            }
        }
    }
}