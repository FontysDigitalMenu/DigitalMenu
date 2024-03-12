using System.ComponentModel.DataAnnotations.Schema;
using QRCoder;

namespace DigitalMenu_20_BLL.Models;

public class Table
{
    public int Id { get; set; }

    public string Name { get; set; }

    public List<Order> Orders { get; set; }
    
    [NotMapped]
    public string QrCode => GetQrCode(Name);

    private static string GetQrCode(string qrCodeText)
    {
        QRCodeGenerator qrCodeGenerator = new();
        QRCodeData qrCodeData = qrCodeGenerator.CreateQrCode(qrCodeText, QRCodeGenerator.ECCLevel.Q);
        BitmapByteQRCode bitmapByteQrCode = new(qrCodeData);
        byte[] qrCodeAsBitmapByte = bitmapByteQrCode.GetGraphic(20);

        return Convert.ToBase64String(qrCodeAsBitmapByte);
    }
}