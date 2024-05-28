using DigitalMenu_20_BLL.Exceptions;
using DigitalMenu_20_BLL.Interfaces.Repositories;
using DigitalMenu_20_BLL.Interfaces.Services;
using DigitalMenu_20_BLL.Models;

namespace DigitalMenu_20_BLL.Services;

public class SettingService(ISettingRepository settingRepository) : ISettingService
{
    public async Task<Setting?> GetSettings()
    {
        return await settingRepository.GetSettings();
    }

    public async Task<bool> UpdateSettings(Setting setting)
    {
        if (setting.Id == 0)
        {
            throw new NotFoundException("Setting does not exist");
        }

        if (setting.CompanyName == "" || setting.PrimaryColor == "" || setting.SecondaryColor == "")
        {
            throw new ArgumentException("Setting fields cannot be empty");
        }

        return await settingRepository.UpdateSettings(setting);
    }
}