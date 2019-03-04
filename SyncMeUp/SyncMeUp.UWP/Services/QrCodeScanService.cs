using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.UI.Core;
using SyncMeUp.Domain.Services;
using ZXing;
using ZXing.Mobile;

namespace SyncMeUp.UWP.Services
{
    public class QrCodeScanService : IQrCodeScanService
    {
        private readonly CoreDispatcher _dispatcher;
        public QrCodeScanService(CoreDispatcher dispatcher)
        {
            _dispatcher = dispatcher;
        }
        public async Task<byte[]> ScanQrCode()
        {
            var scanner = new MobileBarcodeScanner(_dispatcher) { Dispatcher = _dispatcher };

            var options = MobileBarcodeScanningOptions.Default;
            options.PossibleFormats = new List<BarcodeFormat> { BarcodeFormat.QR_CODE };

            var result = await scanner.Scan(options);

            return result?.RawBytes;
        }
    }
}