﻿using DigitalMenu_20_BLL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalMenu_20_BLL.Interfaces.Services
{
    public interface IIngredientService
    {
        Task<Ingredient?> GetIngredientByNameAsync(string name);
    }
}
