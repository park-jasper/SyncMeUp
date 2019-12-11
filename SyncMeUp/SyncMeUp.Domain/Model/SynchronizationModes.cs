namespace SyncMeUp.Domain.Model
{
    public class SynchronizationModes
    {
        public bool Upload { get; set; }
        public bool Download { get; set; }

        public bool CanEditFiles { get; set; }
        public bool CanDeleteFiles { get; set; }
        public bool CanAddFiles { get; set; }
    }
}