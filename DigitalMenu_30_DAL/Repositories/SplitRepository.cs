using DigitalMenu_20_BLL.Interfaces.Repositories;
using DigitalMenu_20_BLL.Models;
using DigitalMenu_30_DAL.Data;
using Microsoft.EntityFrameworkCore;

namespace DigitalMenu_30_DAL.Repositories;

public class SplitRepository(ApplicationDbContext dbContext) : ISplitRepository
{
    public Split? GetByExternalPaymentId(string id)
    {
        return dbContext.Splits
            .Include(s => s.Order)
            .ThenInclude(o => o.OrderMenuItems)
            .ThenInclude(omi => omi.MenuItem)
            .FirstOrDefault(o => o.ExternalPaymentId == id);
    }

    public bool Update(Split split)
    {
        dbContext.Splits.Update(split);
        return dbContext.SaveChanges() > 0;
    }

    public Split? GetById(int id)
    {
        return dbContext.Splits.Find(id);
    }

    public Split? Create(Split split)
    {
        dbContext.Splits.Add(split);
        return dbContext.SaveChanges() > 0 ? split : null;
    }
}