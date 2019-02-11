using System.Collections.Generic;
using System.Threading.Tasks;
using SyncMeUp.Services;
using ZXing;
using ZXing.Mobile;

namespace SyncMeUp.Droid.Services
{
    public class QrCodeScanService : IQrCodeScanService
    {
        public async Task<byte[]> ScanQrCode()
        {
            var scanner = new MobileBarcodeScanner();
            var options = MobileBarcodeScanningOptions.Default;
            options.PossibleFormats = new List<BarcodeFormat> { BarcodeFormat.QR_CODE };

            var result = await scanner.Scan(options);

            return result?.RawBytes;
        }
    }
}