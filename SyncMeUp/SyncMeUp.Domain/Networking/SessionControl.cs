using System.Collections.Generic;
using System.Threading.Tasks;
using SyncMeUp.Domain.Contracts;
using SyncMeUp.Domain.Model;

namespace SyncMeUp.Domain.Networking
{
    public class SessionControl
    {
        private readonly INetworkStream _stream;
        private readonly byte[] _sessionKey;
        public SessionControl(INetworkStream stream, byte[] sessionKey)
        {
            _stream = stream;
            _sessionKey = sessionKey;
        }

        public async Task<bool> OfferContainer(SynchronizationContainer container)
        {

        }
    }
}