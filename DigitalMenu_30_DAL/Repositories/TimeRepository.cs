using DigitalMenu_20_BLL.Interfaces.Repositories;
using DigitalMenu_20_BLL.Models;
using DigitalMenu_30_DAL.Data;

namespace DigitalMenu_30_DAL.Repositories;

public class TimeRepository(ApplicationDbContext dbContext) : ITimeRepository
{
    public Time? Get()
    {
        return dbContext.Times.FirstOrDefault();
    }

    public bool UpdateOrCreate(Time time)
    {
        Time? existingTime = dbContext.Times.FirstOrDefault();
        if (existingTime == null)
        {
            dbContext.Times.Add(time);
        }
        else
        {
            existingTime.Hours = time.Hours;
            existingTime.Minutes = time.Minutes;
            dbContext.Times.Update(existingTime);
        }

        return dbContext.SaveChanges() > 0;
    }

    public bool Delete()
    {
        Time? time = dbContext.Times.FirstOrDefault();
        if (time == null)
        {
            return false;
        }

        dbContext.Times.Remove(time);
        return dbContext.SaveChanges() > 0;
    }
}