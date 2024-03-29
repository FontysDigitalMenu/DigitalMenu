﻿using DigitalMenu_20_BLL.Models;

namespace DigitalMenu_20_BLL.Interfaces.Services;

public interface ITableService
{
    // public string GenerateQrCode(string backendUrl, string id);

    public List<Table> GetAll();

    public bool Create(Table table);

    public Table? GetById(string id);

    public bool Update(Table table);

    public bool Delete(string id);
}