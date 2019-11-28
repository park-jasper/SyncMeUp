using System;

namespace SyncMeUp.Domain.Domain
{
    [Flags]
    public enum SynchronizationOptions
    {
        Upload = 1,
        Download = 2
    }
}