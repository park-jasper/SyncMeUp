using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using MoreLinq;
using SyncMeUp.Cryptography;

namespace SyncMeUp.Networking
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

        public ServerControl Listen(Action onClientConnected, Action onNetworkDisconnected)
        {
            var tokenSource = new CancellationTokenSource();
            Task.Run(() => ListenInternal(onClientConnected, onNetworkDisconnected, tokenSource.Token));
            return new ServerControl(this, tokenSource);
        }

        private async Task ListenInternal(Action onClientConnected, Action onNetworkDisconnected, CancellationToken token)
        {
            _listener.Start();

            while (!token.IsCancellationRequested)
            {
                var tcpClient = await _listener.AcceptTcpClientAsync();
                Task.Run(() => HandleTcpClientAccepted(tcpClient, onClientConnected, onNetworkDisconnected, token));
            }
        }

        private async Task HandleTcpClientAccepted(TcpClient tcpClient, Action onClientConnected, Action onNetworkDisconnected, CancellationToken token)
        {
            try
            {
                using (var stream = tcpClient.GetStream())
                {
                    if (!await VerifyPresharedKey(stream, token))
                    {
                        return;
                    }
                    await SendGuid(stream, token);
                    var initiationMode = await GetInitiationMode(stream, token);
                    switch (initiationMode)
                    {
                        case InitiationMode.Error:
                            return;
                        case InitiationMode.Otp:
                            await HandleRegistrationOverOtp(stream, token);
                            return;
                        case InitiationMode.Standard:
                            break;
                        default:
                            throw new NotImplementedException($"Forgot new InitiationMode {initiationMode}");
                    }
                    var clientGuidResult = await GetGuid(stream, token);
                    if (!clientGuidResult.Successful)
                    {
                        return;
                    }
                    //Veritfy guid against guid list of clients
                    //Check that server has the public key
                    var clientPublicKey = GetPublicKeyForClient(clientGuidResult.Result);
                    var sessionKey = GenerateSessionKey();
                    await SendSessionKey(stream, clientPublicKey, sessionKey, token);

                    int messageLength = 0;
                    byte[] buffer = new byte[256];
                    while (( messageLength = await stream.ReadAsync(buffer, 0, buffer.Length, token) ) != 0 &&
                           !token.IsCancellationRequested)
                    {
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

        private async Task HandleRegistrationOverOtp(NetworkStream stream, CancellationToken token)
        {
            byte[] buffer = new byte[4];
            int length = await stream.ReadAsync(buffer, 0, buffer.Length, token);
            if (length != buffer.Length)
            {
                return;
            }

            var dataType = ConvertToCommunicationData(buffer);
            if (dataType != CommunicationData.PublicKey)
            {
                return;
            }

            byte[] keyLengths = new byte[8];
            length = await stream.ReadAsync(keyLengths, 0, keyLengths.Length, token);
            if (length != keyLengths.Length)
            {
                return;
            }

            var modulusLength = BitConverter.ToInt32(keyLengths, 0);
            var exponentLength = BitConverter.ToInt32(keyLengths, 4);
            byte[] encryptedModulus = new byte[modulusLength];
            byte[] encryptedPublicKeyExponent = new byte[exponentLength];
            length = await stream.ReadAsync(encryptedModulus, 0, modulusLength, token);
            if (length != modulusLength)
            {
                return;
            }
            var lengthTask = stream.ReadAsync(encryptedPublicKeyExponent, 0, exponentLength, token);
            var blowFish = new BlowFish(CurrentOtp);
            byte[] decryptedModulus = blowFish.Decrypt_CBC(encryptedModulus);
            length = await lengthTask;
            if (length != exponentLength)
            {
                return;
            }
            byte[] decryptedPublicKeyExponent = blowFish.Decrypt_CBC(encryptedPublicKeyExponent);
            var clientPublicKey = new RsaPublicKey(decryptedModulus, decryptedPublicKeyExponent);
            throw new NotImplementedException();
        }

        private static async Task<bool> VerifyPresharedKey(NetworkStream stream, CancellationToken token)
        {
            byte[] buffer = new byte[Networking.PresharedKey.Length];
            int messageLength = await stream.ReadAsync(buffer, 0, buffer.Length, token);
            return messageLength == Networking.PresharedKey.Length 
                   && buffer
                       .EquiZip(Networking.PresharedKey, (left, right) => left == right)
                       .All(eq => eq);
        }

        private static async Task<InitiationMode> GetInitiationMode(NetworkStream stream, CancellationToken token)
        {
            byte[] buffer = new byte[4];
            int messageLength = await stream.ReadAsync(buffer, 0, buffer.Length, token);
            if (messageLength != 4)
            {
                return InitiationMode.Error;
            }

            return ConvertToInitiationMode(buffer);
        }

        private RsaPublicKey GetPublicKeyForClient(Guid clientGuid)
        {
            throw new NotImplementedException();
        }

        private byte[] GenerateSessionKey()
        {
            byte[] key = new byte[BlowFish.MaxKeyLength];
            _randomSource.GetBytes(key);
            return key;
        }

        private async Task SendSessionKey(NetworkStream stream, RsaPublicKey clientPublicKey, byte[] sessionKey, CancellationToken token)
        {
            var encryptedSessionKey = RsaHelper.Encrypt(clientPublicKey, sessionKey);
            await stream.WriteAsync(CommunicationData.SessionKey.ToByteArray(), 0, 4, token);
            await stream.WriteAsync(BitConverter.GetBytes(encryptedSessionKey.Length), 0, 4, token);
            await stream.WriteAsync(encryptedSessionKey, 0, encryptedSessionKey.Length, token);
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