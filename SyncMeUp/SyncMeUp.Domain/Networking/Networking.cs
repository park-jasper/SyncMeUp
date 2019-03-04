using System.Net.Sockets;

namespace SyncMeUp.Domain.Networking
{
    public static class Networking
    {
        public static byte[] PresharedKey =
        {
            15, 137, 190, 208, 5, 43, 41, 192, 62, 31, 70, 242, 72, 54, 68, 129, 157, 119, 152, 147, 173, 118, 89, 186,
            254, 124, 69, 153, 69, 6, 95, 227, 151, 125, 109, 92, 233, 154, 24, 201
        };
        public const AddressFamily AddressFamilyToUse = AddressFamily.InterNetwork;
    }
}