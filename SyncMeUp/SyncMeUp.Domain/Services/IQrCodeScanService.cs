using System.Threading.Tasks;

namespace SyncMeUp.Domain.Services
{
    public interface IQrCodeScanService
    {
        Task<byte[]> ScanQrCode();
    }
}