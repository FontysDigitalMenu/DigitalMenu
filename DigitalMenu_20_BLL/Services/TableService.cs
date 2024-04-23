using DigitalMenu_20_BLL.Exceptions;
using DigitalMenu_20_BLL.Interfaces.Repositories;
using DigitalMenu_20_BLL.Interfaces.Services;
using DigitalMenu_20_BLL.Models;

// using QRCoder;

namespace DigitalMenu_20_BLL.Services;

public class TableService(ITableRepository tableRepository) : ITableService
{
    // public string GenerateQrCode(string backendUrl, string id)
    // {
    //     QRCodeGenerator qrCodeGenerator = new();
    //     QRCodeData qrCodeData = qrCodeGenerator.CreateQrCode($"{backendUrl}/table/{id}", QRCodeGenerator.ECCLevel.Q);
    //     BitmapByteQRCode bitmapByteQrCode = new(qrCodeData);
    //     byte[] qrCodeAsBitmapByte = bitmapByteQrCode.GetGraphic(20);
    //
    //     return Convert.ToBase64String(qrCodeAsBitmapByte);
    // }

    public List<Table> GetAll()
    {
        return tableRepository.GetAll();
    }

    public Table? Create(Table table)
    {
        return tableRepository.Create(table);
    }

    public Table? GetById(string id)
    {
        return tableRepository.GetById(id);
    }

    public bool Update(Table table)
    {
        return tableRepository.Update(table);
    }

    public bool Delete(string id)
    {
        return tableRepository.Delete(id);
    }
    
    public bool ResetSession(string id)
    {
        Table? table = tableRepository.GetById(id);
        
        if (table == null)
        {
            throw new NotFoundException("TableId does not exist");
        }
        
        table.HostId = null;
        table.SessionId = Guid.NewGuid().ToString();
        
        return tableRepository.Update(table);
    }
    
    public bool AddHost(string id, string deviceId)
    {
        Table? table = tableRepository.GetById(id);
        
        if (table == null)
        {
            throw new NotFoundException("TableId does not exist");
        }
        
        if (table.HostId == null)
        {
            table.HostId = deviceId;
            return tableRepository.Update(table);
        }
        return false;
    }
}