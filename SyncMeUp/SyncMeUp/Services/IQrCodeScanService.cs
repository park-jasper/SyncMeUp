using System.Threading.Tasks;

namespace SyncMeUp.Services
{
    public interface IQrCodeScanService
    {
        Task<byte[]> ScanQrCode();
    }
}