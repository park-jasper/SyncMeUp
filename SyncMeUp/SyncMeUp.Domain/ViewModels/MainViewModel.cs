using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Windows.Input;
using QRCoder;
using SyncMeUp.Domain.Commands;
using SyncMeUp.Domain.Contracts;
using SyncMeUp.Domain.Cryptography;
using SyncMeUp.Domain.Domain;
using SyncMeUp.Domain.Networking;
using SyncMeUp.Domain.Services;

namespace SyncMeUp.Domain.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        public byte[] QrCodeImageData { get; set; }

        public string Info { get; set; }

        public bool GuiEnabled { get; set; }

        public ICommand ScanQrCode { get; set; }
        public ICommand MakeOtp { get; set; }
        public ICommand StopServer { get; set; }
        public ICommand Test { get; set; }

        public ContainerViewModel Container { get; set; } = new ContainerViewModel(new KnownClientsProvider(Di.GetInstance<ISecureStorageProvider>()), new SynchronizationContainer
        {
            Name = "Musik",
            RelativePath = @"E:\Jasper\Music",
            Guid = Guid.NewGuid(),
            SyncOptions = SynchronizationOptions.Download | SynchronizationOptions.Upload,
            Files = new List<string>(),
            KnownPeers = new List<Guid>()
        });

        public MainViewModel()
        {
            ScanQrCode = new CommandForwarding(sender => ClickButton());
            MakeOtp = new CommandForwarding(sender => MakeClientRegister());
            StopServer = new CommandForwarding(sender => _serverControl?.Stop());
            Test = new CommandForwarding(async sender =>
            {
                //var storage = Di.GetInstance<ISecureStorageProvider>();
                //await storage.SetAsync("SomeKey", "Wololo");
                var provider = Di.GetInstance<IBulkStorageProvider>();
                var bulk = provider.OpenBulkStorage();
                var f = bulk.OpenFile("testTable");

                await Task.Delay(5000);
                provider.CloseBulkStorage(bulk);
            });

            GuiEnabled = true;
        }

        private async void ClickButton()
        {
            var storageProvider = Di.GetInstance<ISecureStorageProvider>();
            GuiEnabled = false;
            var keyPair = RsaKeyPair.Deserialize(await storageProvider.GetAsync(RsaHelper.RsaDeviceKeyPairIdentifier));
            if (keyPair == null)
            {
                keyPair = await Task.Run(() => RsaHelper.GenerateRsaKeyPair());
                await storageProvider.SetAsync(RsaHelper.RsaDeviceKeyPairIdentifier, RsaKeyPair.Serialize(keyPair));
            }
            var scanner = Di.GetInstance<IQrCodeScanService>();

            var result = await scanner.ScanQrCode();

            if (result != null)
            {
                var guid = new Guid("19637d77-92ba-4c18-ade5-227f9fcd3e07");
                var client = new Client(guid, keyPair);

                var gLength = new Guid().ToByteArray().Length;
                var serverGuid = new byte[gLength];
                var ipLength = 4;
                var ipAddress = new byte[ipLength];
                var otp = new byte[result.Length - gLength - ipLength];

                Array.Copy(result, serverGuid, gLength);
                Array.Copy(result, gLength, ipAddress, 0, ipLength);
                Array.Copy(result, gLength + ipLength, otp, 0, otp.Length);
                var control = client.RegisterWithServer(new IPAddress(ipAddress), 1585, new Guid(serverGuid), otp);
                var connectionResult = await control.ConnectionTask;
            }
            GuiEnabled = true;
        }

        private Server.ServerControl _serverControl;

        private void MakeClientRegister()
        {
            var guid = new Guid("b0ef7dda-ad9a-4354-aec0-21447e84bf9d");
            var server = Server.CreateServer(1585, guid, Di.GetInstance<ISettingsProvider>());

            var randomSource = new RNGCryptoServiceProvider();

            var guidByteArray = guid.ToByteArray();
            var gLength = guidByteArray.Length;
            var ipAddressByteArray = Server.GetLocalIpAddress().GetAddressBytes();
            var ipLength = ipAddressByteArray.Length;
            var otp = new byte[BlowFish.MaxKeyLength];
            randomSource.GetBytes(otp);
            var combined = new byte[gLength + ipLength + otp.Length];
            Array.Copy(guidByteArray, combined, gLength);
            Array.Copy(ipAddressByteArray, 0, combined, gLength, ipLength);
            Array.Copy(otp, 0, combined, gLength + ipLength, otp.Length);
            MakeQrCode(combined);

            server.SetNewOtp(otp);
            _serverControl = server.Listen(() => Info = "Client connected", () => { });
        }


        public void MakeQrCode(byte[] content)
        {
            MakeQrCode(Convert.ToBase64String(content));
        }

        public void MakeQrCode(string content)
        {
            var gen = new QRCodeGenerator();
            var data = gen.CreateQrCode(content, QRCodeGenerator.ECCLevel.H);
            var code = new BitmapByteQRCode(data);
            QrCodeImageData = code.GetGraphic(20);
        }
    }
}