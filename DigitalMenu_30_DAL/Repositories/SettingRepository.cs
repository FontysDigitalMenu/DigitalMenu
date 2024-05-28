using DigitalMenu_20_BLL.Interfaces.Repositories;
using DigitalMenu_20_BLL.Models;
using DigitalMenu_30_DAL.Data;
using Microsoft.EntityFrameworkCore;

namespace DigitalMenu_30_DAL.Repositories;

public class SettingRepository(ApplicationDbContext dbContext) : ISettingRepository
{
    public Task<Setting?> GetSettings()
    {
        return dbContext.Settings.FirstOrDefaultAsync();
    }

    public async Task<bool> UpdateSettings(Setting setting)
    {
        dbContext.Settings.Update(setting);
        return await dbContext.SaveChangesAsync() > 0;
    }
}