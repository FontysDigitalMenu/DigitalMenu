using DigitalMenu_20_BLL.Models;

namespace DigitalMenu_20_BLL.Interfaces.Services;

public interface ITableService
{
    // public string GenerateQrCode(string backendUrl, string id);

    public List<Table> GetAll();

    public Table? Create(Table table);

    public Table? GetById(string id);

    public bool Update(Table table);

    public bool Delete(string id);
    
    public bool ResetSession(string id);
    
    public bool AddHost(string id, string deviceId);
}