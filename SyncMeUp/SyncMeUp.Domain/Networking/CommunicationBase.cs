using System;
using System.Security.Cryptography;
using System.Threading;

namespace SyncMeUp.Domain.Networking
{
    public abstract class CommunicationBase
    {
        public Guid Guid { get; }
        protected RNGCryptoServiceProvider _randomSource = new RNGCryptoServiceProvider();

        protected CommunicationBase(Guid guid)
        {
            Guid = guid;
        }

        public abstract class CommunicationsControl
        {
            protected CancellationTokenSource TokenSource { get; }
            protected CommunicationsControl(CancellationTokenSource tokenSource)
            {
                TokenSource = tokenSource;
            }

            public virtual void Stop()
            {
                TokenSource.Cancel();
            }
        }
    }
}