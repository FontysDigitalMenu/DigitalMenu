using DigitalMenu_20_BLL.Interfaces.Repositories;
using DigitalMenu_20_BLL.Models;
using DigitalMenu_30_DAL.Data;

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

    public bool Update(Table table)
    {
        dbContext.Tables.Update(table);
        return dbContext.SaveChanges() > 0;
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
}