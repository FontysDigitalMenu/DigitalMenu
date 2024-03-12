using QRCoder;

namespace DigitalMenu_20_BLL.Models;

public class Table
{
    public int Id { get; set; }

    public string Name { get; set; }

    public List<Order> Orders { get; set; }

    public string GetQrCode(string apiUrl)
    {
        QRCodeGenerator qrCodeGenerator = new();
        QRCodeData qrCodeData = qrCodeGenerator.CreateQrCode($"{apiUrl}/{Id}", QRCodeGenerator.ECCLevel.Q);
        BitmapByteQRCode bitmapByteQrCode = new(qrCodeData);
        byte[] qrCodeAsBitmapByte = bitmapByteQrCode.GetGraphic(20);

        return Convert.ToBase64String(qrCodeAsBitmapByte);
    }
}