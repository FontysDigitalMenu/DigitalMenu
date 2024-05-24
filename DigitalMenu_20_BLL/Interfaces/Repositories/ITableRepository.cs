using DigitalMenu_20_BLL.Models;

namespace DigitalMenu_20_BLL.Interfaces.Repositories;

public interface ITableRepository
{
    public List<Table> GetAll();

    public Table? Create(Table table);

    public Table? GetById(string id);

    public Table? GetBySessionId(string sessionId);

    public bool Update(Table table);

    public bool Delete(string id);

    public Table? GetTableByIdWithReservationsFromDay(string id, DateTime dateTime);
    
    public Table? GetTableBySessionIdWithReservationsFromDay(string sessionId, DateTime dateTime);

    public List<Table> GetAllReservableTablesWithReservationsFrom(DateTime dateTime);
}