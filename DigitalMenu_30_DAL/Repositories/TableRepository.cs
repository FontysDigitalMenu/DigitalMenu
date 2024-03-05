using DigitalMenu_20_BLL.Interfaces.Repositories;
using DigitalMenu_20_BLL.Models;
using DigitalMenu_30_DAL.Data;

namespace DigitalMenu_30_DAL.Repositories;

public class TableRepository : ITableRepository
{
    private readonly ApplicationDbContext _dbContext;

    public TableRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public List<Table> GetAll()
    {
        return _dbContext.Tables.ToList();
    }

    public bool Create(Table table)
    {
        _dbContext.Tables.Add(table);
        return _dbContext.SaveChanges() > 0;
    }

    public Table? GetById(int id)
    {
        return _dbContext.Tables.Find(id);
    }

    public bool Update(Table table)
    {
        _dbContext.Tables.Update(table);
        return _dbContext.SaveChanges() > 0;
    }

    public bool Delete(int id)
    {
        Table? table = _dbContext.Tables.Find(id);
        if (table == null)
        {
            return false;
        }

        _dbContext.Tables.Remove(table);
        return _dbContext.SaveChanges() > 0;
    }
}