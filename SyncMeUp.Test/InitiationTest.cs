using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using MoreLinq;
using SyncMeUp.Domain.Cryptography;
using SyncMeUp.Domain.Networking;
using SyncMeUp.Test.Contracts;
using SyncMeUp.Test.Mocking;

namespace SyncMeUp.Test
{
    public class InitiationTest
    {
        public IAssert Assert { get; set; }
        public InitiationTest(IAssert assert)
        {
            Assert = assert;
        }
        public async Task TestOtpInitiation()
        {
            var serverStreamMock = new NetworkStreamMock() {Name = "ServerMock"};
            var clientStreamMock = new NetworkStreamMock() {Name = "ClientMock"};
            NetworkStreamMock.Link(serverStreamMock, clientStreamMock);
            var tokenSource = new CancellationTokenSource();
            var token = tokenSource.Token;
            var serverGuid = new Guid("666b8b49-dc20-4aac-aacb-493c41dbbfb4");
            var clientGuid = new Guid("165dad67-8dc7-402d-a70f-1548c3587a6a");
            var otp = new byte[]
            {
                111, 153, 155, 45, 0, 122, 205, 121, 187, 19, 71, 123, 9, 82, 33, 130, 25, 4, 113, 148, 40, 34, 141, 64,
                76, 9, 2, 180, 140, 26, 192, 180, 119, 31, 180, 175, 93, 81, 114, 59, 0, 183, 25, 164, 161, 126, 84,
                158, 195, 115, 16, 165, 224, 205, 44, 6
            };
            var clientKeys = RsaTestHelper.TestKeyPair;
            var intent = InitiationIntent.GetOtpInitiationIntent(serverGuid, otp, clientKeys.PublicKey);
            RsaPublicKey transferredPublicKey = new RsaPublicKey(new byte[0], new byte[0]);
            var serverTask = Task.Run(() => InitializationHandler.HandleInitializationOfClient(serverStreamMock, serverGuid, () => otp,
                guid => clientKeys.PublicKey, (guid, key) => { transferredPublicKey = key; }, token), token);
            var clientTask = Task.Run(() => InitializationHandler.ConnectToServer(clientStreamMock, clientGuid, intent, token), token);
            var serverResult = await serverTask;
            var clientResult = await clientTask;

            Assert.IsTrue(serverResult.Successful);
            Assert.IsTrue(clientResult.Successful);

            Assert.IsTrue(clientKeys.PublicKey.PublicKeyExponent.EquiZip(transferredPublicKey.PublicKeyExponent, (left, right) => left == right).All(eq => eq));
        }
    }
}
