using DigitalMenu_20_BLL.Interfaces.Repositories;
using DigitalMenu_20_BLL.Models;
using DigitalMenu_30_DAL.Data;
using Microsoft.EntityFrameworkCore;

namespace DigitalMenu_30_DAL.Repositories;

public class TableRepository(ApplicationDbContext dbContext) : ITableRepository
{
    public List<Table> GetAll()
    {
        return dbContext.Tables.OrderBy(t => t.CreatedAt).ToList();
    }

    public Table? Create(Table table)
    {
        table.CreatedAt = DateTime.Now;
        dbContext.Tables.Add(table);
        return dbContext.SaveChanges() > 0 ? table : null;
    }

    public Table? GetById(string id)
    {
        return dbContext.Tables.Find(id);
    }

    public Table? GetBySessionId(string sessionId)
    {
        return dbContext.Tables.FirstOrDefault(t => t.SessionId == sessionId);
    }

    public bool Update(Table table)
    {
        dbContext.Tables.Update(table);
        dbContext.SaveChanges();
        return true;
    }

    public bool Delete(string id)
    {
        Table? table = dbContext.Tables.Find(id);
        if (table == null)
        {
            return false;
        }

        dbContext.Tables.Remove(table);
        return dbContext.SaveChanges() > 0;
    }

    public List<Table> GetAllReservableTablesWithReservationsFrom(DateTime dateTime)
    {
        return dbContext.Tables
            .Include(t => t.Reservations.Where(r => r.ReservationDateTime.Date == dateTime.Date))
            .Where(t => t.IsReservable)
            .ToList();
    }
}