using System.Collections.Generic;
using Newtonsoft.Json;

namespace SyncMeUp.Domain.Model
{
    public class SynchronizationFolder
    {
        public string Name { get; set; }
        [JsonIgnore]
        public SynchronizationFolder Parent { get; set; }
        public IList<SynchronizationFolder> Folders { get; set; }
        public IList<SynchronizationFile> Files { get; set; }

        public SynchronizationFolder()
        {
            Folders = new List<SynchronizationFolder>();
            Files = new List<SynchronizationFile>();
        }

        public override string ToString()
        {
            return $"{Name} - Folders: {Folders.Count}, Files:{Files.Count}";
        }
    }
}