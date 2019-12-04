using System;
using System.Collections.Generic;

namespace SyncMeUp.Domain.Domain
{
    public class SynchronizationContainer
    {
        public Guid Guid { get; set; }
        public string Name { get; set; }
        public string RelativePath { get; set; }
        public IList<string> Files { get; set; }
        public SynchronizationModes SyncModes { get; set; }
        public CommunicationRole OwnCommunicationRole { get; set; }
        public IList<Guid> KnownPeers { get; set; }
    }
}