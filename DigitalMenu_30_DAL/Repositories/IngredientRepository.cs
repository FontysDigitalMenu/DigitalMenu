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

    public async Task<bool> DeleteIngredientsByMenuItemId(int menuItemId)
    {
        List<MenuItemIngredient> ingredientMenuItems = await dbContext.MenuItemIngredients
            .Where(cmi => cmi.MenuItemId == menuItemId)
            .ToListAsync();

        if (ingredientMenuItems.Count == 0)
        {
            return true;
        }

        dbContext.MenuItemIngredients.RemoveRange(ingredientMenuItems);
        await dbContext.SaveChangesAsync();

        return true;
    }

    public async Task<Ingredient?> CreateIngredient(Ingredient ingredient)
    {
        dbContext.Ingredients.Add(ingredient);
        return await dbContext.SaveChangesAsync() > 0 ? ingredient : null;
    }

    public async Task<Ingredient?> GetIngredientById(int ingredientId)
    {
        return await dbContext.Ingredients.FindAsync(ingredientId);
    }

    public async Task<bool> UpdateIngredient(Ingredient ingredient)
    {
        dbContext.Ingredients.Update(ingredient);
        return await dbContext.SaveChangesAsync() > 0;
    }
}