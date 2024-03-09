using DigitalMenu_20_BLL.Interfaces.Repositories;
using DigitalMenu_20_BLL.Interfaces.Services;
using DigitalMenu_20_BLL.Models;

namespace DigitalMenu_20_BLL.Services;

public class TableService : ITableService
{
    private readonly ITableRepository _tableRepository;

    public TableService(ITableRepository tableRepository)
    {
        _tableRepository = tableRepository;
    }

    public List<Table> GetAll()
    {
        return _tableRepository.GetAll();
    }
    
    public bool Create(Table table)
    {
        return _tableRepository.Create(table);
    }

    public Table? GetById(int id)
    {
        return _tableRepository.GetById(id);
    }
    
    public bool Update(Table table)
    {
        return _tableRepository.Update(table);
    }
    
    public bool Delete(int id)
    {
        return _tableRepository.Delete(id);
    }
}