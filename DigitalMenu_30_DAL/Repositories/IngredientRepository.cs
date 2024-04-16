using DigitalMenu_20_BLL.Interfaces.Repositories;
using DigitalMenu_20_BLL.Models;
using DigitalMenu_30_DAL.Data;
using Microsoft.EntityFrameworkCore;

namespace DigitalMenu_30_DAL.Repositories;

public class IngredientRepository(ApplicationDbContext dbContext) : IIngredientRepository
{
    public async Task<Ingredient?> GetIngredientByNameAsync(string name)
    {
        return await dbContext.Ingredients
            .FirstOrDefaultAsync(i => i.Name == name);
    }

    public async Task<List<Ingredient>> GetIngredients()
    {
        return await dbContext.Ingredients
            .OrderBy(i => i.Id)
            .ToListAsync();
    }
}