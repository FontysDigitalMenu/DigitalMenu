using DigitalMenu_20_BLL.Models;

namespace DigitalMenu_20_BLL.Interfaces.Services;

public interface ISettingService
{
    public Task<Setting?> GetSettings();

    public Task<bool> UpdateSettings(Setting setting);
}