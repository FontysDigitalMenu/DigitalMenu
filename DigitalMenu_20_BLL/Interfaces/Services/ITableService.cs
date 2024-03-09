using DigitalMenu_20_BLL.Models;

namespace DigitalMenu_20_BLL.Interfaces.Services;

public interface ITableService
{
    public List<Table> GetAll();
    
    public bool Create(Table table);
    
    public Table? GetById(int id);
    
    public bool Update(Table table);
    
    public bool Delete(int id);
}