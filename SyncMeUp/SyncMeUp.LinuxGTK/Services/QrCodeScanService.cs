using System.Threading.Tasks;
using SyncMeUp.Domain.Services;

namespace SyncMeUp.LinuxGTK.Services
{
    public class QrCodeScanService : IQrCodeScanService
    {
        public Task<byte[]> ScanQrCode()
        {
            throw new System.NotImplementedException();
        }
    }
}