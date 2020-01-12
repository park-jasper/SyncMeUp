using System;
using System.Collections.Generic;

namespace SyncMeUp.Domain.Model
{
    public class SynchronizationContainer
    {
        public Guid Guid { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public SynchronizationFolder Content { get; set; }
        public SynchronizationModes SyncModes { get; set; } = new SynchronizationModes();
        public CommunicationRole OwnCommunicationRole { get; set; }
        public IList<Guid> KnownPeers { get; set; } = new List<Guid>();
    }
}