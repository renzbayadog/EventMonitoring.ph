using QRCoder;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using ZXing.QrCode.Internal;

namespace EventMonitoring.ph.Services
{
    public interface IQRCodeService
    {
        string GenerateQRCode(string text, string color = "#000000");
    }

    public class QRCodeService : IQRCodeService
    {
        public string GenerateQRCode(string text, string color = "#000000")
        {
            if (string.IsNullOrWhiteSpace(text))
                return string.Empty;

            //var qrGenerator = new QRCodeGenerator();
            //using var qrCodeData = qrGenerator.CreateQrCode(text, QRCodeGenerator.ECCLevel.L);
            //using var qrCode = new QRCode(qrCodeData);
            //using var qrCodeImage = qrCode.GetGraphic(20, ColorTranslator.FromHtml(color), Color.White, true);

            //using var ms = new MemoryStream();
            //qrCodeImage.Save(ms, ImageFormat.Png);
            //var imageBytes = ms.ToArray();

            using var qrGen = new QRCodeGenerator();
            using var qrData = qrGen.CreateQrCode(text, QRCodeGenerator.ECCLevel.L);
            using var qrCode = new PngByteQRCode(qrData);
            byte[] imgBytes = qrCode.GetGraphic(20, ColorTranslator.FromHtml(color), Color.White, true);
            return $"data:image/png;base64,{Convert.ToBase64String(imgBytes)}";

           // return $"data:image/png;base64,{Convert.ToBase64String(imageBytes)}";
        }
    }
} 