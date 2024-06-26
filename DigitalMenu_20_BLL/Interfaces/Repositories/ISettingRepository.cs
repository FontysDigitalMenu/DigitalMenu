﻿using DigitalMenu_20_BLL.Models;

namespace DigitalMenu_20_BLL.Interfaces.Repositories;

public interface ISettingRepository
{
    public Task<Setting?> GetSettings();

    public Task<bool> UpdateSettings(Setting setting);
}