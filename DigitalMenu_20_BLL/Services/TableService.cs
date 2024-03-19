using DigitalMenu_20_BLL.Interfaces.Repositories;
using DigitalMenu_20_BLL.Interfaces.Services;
using DigitalMenu_20_BLL.Models;
using QRCoder;

namespace DigitalMenu_20_BLL.Services;

public class TableService : ITableService
{
    private readonly ITableRepository _tableRepository;

    public TableService(ITableRepository tableRepository)
    {
        _tableRepository = tableRepository;
    }

    public string GenerateQrCode(string backendUrl, string id)
    {
        QRCodeGenerator qrCodeGenerator = new();
        QRCodeData qrCodeData = qrCodeGenerator.CreateQrCode($"{backendUrl}/table/{id}", QRCodeGenerator.ECCLevel.Q);
        BitmapByteQRCode bitmapByteQrCode = new(qrCodeData);
        byte[] qrCodeAsBitmapByte = bitmapByteQrCode.GetGraphic(20);

        return Convert.ToBase64String(qrCodeAsBitmapByte);
    }

    public List<Table> GetAll()
    {
        return _tableRepository.GetAll();
    }

    public bool Create(Table table)
    {
        return _tableRepository.Create(table);
    }

    public Table? GetById(string id)
    {
        return _tableRepository.GetById(id);
    }

    public bool Update(Table table)
    {
        return _tableRepository.Update(table);
    }

    public bool Delete(string id)
    {
        return _tableRepository.Delete(id);
    }
}