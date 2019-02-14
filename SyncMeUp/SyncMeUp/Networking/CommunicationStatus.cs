using System;

namespace SyncMeUp.Networking
{
    public enum CommunicationStatus
    {

    }

    public enum InitiationMode
    {
        Error = 0,
        Standard = 10,
        Otp = 20,
    }

    public enum CommunicationData
    {
        Error = 0,
        None = 10,
        PublicKey = 20,
        SessionKey = 30,
    }

    public static class CommunicationStatusExtensions
    {
        public static byte[] ToByteArray(this InitiationMode initiationMode)
        {
            var value = (int) initiationMode;
            return BitConverter.GetBytes(value);
        }
        public static byte[] ToByteArray(this CommunicationData communicationData)
        {
            var value = (int) communicationData;
            return BitConverter.GetBytes(value);
        }
    }
}