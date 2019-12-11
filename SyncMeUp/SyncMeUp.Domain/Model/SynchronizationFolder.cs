using System.Collections.Generic;

namespace SyncMeUp.Domain.Model
{
    public class SynchronizationFolder
    {
        public string Name { get; set; }
        public IList<SynchronizationFolder> Folders { get; set; }
        public IList<SynchronizationFile> Files { get; set; }

        public SynchronizationFolder()
        {
            Folders = new List<SynchronizationFolder>();
            Files = new List<SynchronizationFile>();
        }
    }
}