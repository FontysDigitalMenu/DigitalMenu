using DigitalMenu_20_BLL.Models;
using DigitalMenu_30_DAL.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DigitalMenu_20_BLL.Interfaces.Repositories;

namespace DigitalMenu_30_DAL.Repositories
{
    public class IngredientRepository : IIngredientRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public IngredientRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Ingredient?> GetIngredientByNameAsync(string name)
        {
            return await _dbContext.Ingredients
                .FirstOrDefaultAsync(i => i.Name == name);
        }
    }
}
