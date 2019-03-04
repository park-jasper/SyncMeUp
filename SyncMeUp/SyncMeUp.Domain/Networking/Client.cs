using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using SyncMeUp.Domain.Cryptography;
using SyncMeUp.Domain.Services;

namespace SyncMeUp.Domain.Networking
{
    public class Client : CommunicationBase
    {
        private readonly RsaKeyPair _clientKeyPair;
        public Client(Guid clientGuid, RsaKeyPair clientKeyPair = null) : base(clientGuid)
        {
            if (clientKeyPair == null)
            {
                clientKeyPair = RsaHelper.GenerateRsaKeyPair();
            }
            _clientKeyPair = clientKeyPair;
        }

        public ClientControl<bool> RegisterWithServer(IPAddress serverIpAddress, int port, Guid serverGuid, byte[] serverOtp)
        {
            var client = new TcpClient(Networking.AddressFamilyToUse);
            var tokenSource = new CancellationTokenSource();
            var registrationTask = 
                Task.Run(() => RegisterWithServerInternal(client, serverIpAddress, port, serverGuid, serverOtp, tokenSource.Token));
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

        private async Task<object> ConnectToServerInternal(TcpClient client, IPAddress serverIpAddress, int port, CancellationToken token)
        {
            await client.ConnectAsync(serverIpAddress, port);
            var stream = client.GetStream();
            var result = await InitializationHandler.ConnectToServer(
                new RealNetworkStream(stream),
                Guid,
                InitiationIntent.GetStandardInitiationIntent(_clientKeyPair.PrivateKey),
                token);

            throw new NotImplementedException();
        }

        private async Task<bool> RegisterWithServerInternal(TcpClient client, IPAddress serverIpAddress, int port, Guid serverGuid, byte[] serverOtp, CancellationToken token)
        {
            await client.ConnectAsync(serverIpAddress, port);
            using (var stream = client.GetStream())
            {
                var result = await InitializationHandler.ConnectToServer(
                    new RealNetworkStream(stream), 
                    Guid, 
                    InitiationIntent.GetOtpInitiationIntent(
                        serverGuid,
                        serverOtp,
                        _clientKeyPair.PublicKey),
                    token);
                if (!result.Successful)
                {
                    Debug.WriteLine(result.Exception);
                }
            }

            client?.Close();
            return true;
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