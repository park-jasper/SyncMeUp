namespace SyncMeUp.Domain.Networking
{
    public class InitiationResult
    {
        public InitiationMode Mode { get; set; }
        public byte[] SessionKey { get; set; }
    }
}