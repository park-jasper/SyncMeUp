using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using SyncMeUp.Domain.Cryptography;
using SyncMeUp.Domain.Services;

namespace SyncMeUp.Domain.Networking
{
    public class Server : CommunicationBase
    {
        public static bool IsActualLocalIpAddress(IPAddress ipAddress)
        {
            var isIpv4 = ipAddress.AddressFamily == Networking.AddressFamilyToUse;
            var isLoopback = ipAddress.GetAddressBytes()[3] == 1;
            return isIpv4 && !isLoopback;
        }

        public static IPAddress GetLocalIpAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            return host.AddressList.FirstOrDefault(IsActualLocalIpAddress);
        }

        public static Server CreateServer(int port, Guid serverGuid)
        {
            var localIpAddress = GetLocalIpAddress();
            if (localIpAddress != null)
            {
                return new Server(serverGuid, localIpAddress, port);
            }
            else
            {
                throw new Exception("Not connected to any network");
            }
        }

        public IPAddress LocalIpAddress { get; }

        private readonly TcpListener _listener;
        public byte[] CurrentOtp { get; private set; }

        private Server(Guid serverGuid, IPAddress localIpAddress, int port) : base(serverGuid)
        {
            LocalIpAddress = localIpAddress;
            _listener = new TcpListener(LocalIpAddress, port);
        }

        public void SetNewOtp(byte[] otp)
        {
            CurrentOtp = new byte[otp.Length];
            Array.Copy(otp, CurrentOtp, otp.Length);
        }

        public ServerControl Listen(Action onClientConnected, Action onNetworkDisconnected)
        {
            var tokenSource = new CancellationTokenSource();
            Task.Run(() => ListenInternal(onClientConnected, onNetworkDisconnected, tokenSource.Token), tokenSource.Token);
            return new ServerControl(this, tokenSource);
        }

        private async Task ListenInternal(Action onClientConnected, Action onNetworkDisconnected,
            CancellationToken token)
        {
            _listener.Start();

            while (!token.IsCancellationRequested)
            {
                var tcpClient = await _listener.AcceptTcpClientAsync();
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                Task.Run(() => HandleTcpClientAccepted(tcpClient, onClientConnected, onNetworkDisconnected, token), token);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            }
        }

        private async Task HandleTcpClientAccepted(TcpClient tcpClient, Action onClientConnected,
            Action onNetworkDisconnected, CancellationToken token)
        {
            try
            {
                using (var stream = tcpClient.GetStream())
                {
                    var initializationResult =
                        await InitializationHandler.HandleInitializationOfClient(
                            new RealNetworkStream(stream),
                            Guid,
                            () => CurrentOtp,
                            guid => throw new NotImplementedException(),
                            (guid, key) => Console.WriteLine(guid),
                            token);

                    int messageLength = 0;
                    byte[] buffer = new byte[256];
                    while (!token.IsCancellationRequested)
                    {
                        try
                        {
                            messageLength = await stream.ReadAsync(buffer, 0, buffer.Length, token);
                        }
                        catch (Exception exc)
                        {
                            Debug.WriteLine(exc);
                            return;
                        }

                        onClientConnected();
                        throw new NotImplementedException();
                        //actual communication
                    }
                }
            }
            finally
            {
                tcpClient?.Close();
            }
        }


        private RsaPublicKey GetPublicKeyForClient(Guid clientGuid)
        {
            throw new NotImplementedException();
        }

        public class ServerControl : CommunicationsControl
        {
            private readonly Server _server;

            public ServerControl(Server server, CancellationTokenSource tokenSource) : base(tokenSource)
            {
                _server = server;
            }

            public void SetCurrentOtp(byte[] serverOtp)
            {
                _server.CurrentOtp = serverOtp;
            }
        }
    }
}