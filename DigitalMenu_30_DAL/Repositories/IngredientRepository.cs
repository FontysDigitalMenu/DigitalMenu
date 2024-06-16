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
            .Include(i => i.Translations)
            .OrderBy(i => i.Id)
            .ToListAsync();
    }

    public async Task<List<Ingredient>> GetIngredientsPerPage(int lastIngredient, int amount)
    {
        return await dbContext.Ingredients
            .Include(i => i.Translations)
            .OrderBy(i => i.Id)
            .Skip(lastIngredient)
            .Take(amount)
            .ToListAsync();
    }

    public int GetIngredientCount()
    {
        return dbContext.Ingredients
            .Count();
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
        return await dbContext.Ingredients
            .Include(i => i.Translations)
            .FirstOrDefaultAsync(i => i.Id == ingredientId);
    }

    public async Task<bool> UpdateIngredient(Ingredient ingredient)
    {
        Ingredient? trackedIngredient = await dbContext.Ingredients.FindAsync(ingredient.Id);

        if (trackedIngredient != null)
        {
            trackedIngredient.Name = ingredient.Name;
            trackedIngredient.Stock = ingredient.Stock;
        }
        else
        {
            dbContext.Ingredients.Update(ingredient);
        }

        await dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteIngredient(int ingredientId)
    {
        Ingredient? ingredient = await dbContext.Ingredients.FindAsync(ingredientId);
        if (ingredient == null)
        {
            return false;
        }

        dbContext.Ingredients.Remove(ingredient);
        return await dbContext.SaveChangesAsync() > 0;
    }

    public void CreateIngredientTranslations(List<IngredientTranslation> ingredientTranslations)
    {
        dbContext.IngredientTranslations.AddRange(ingredientTranslations);
        dbContext.SaveChanges();
    }

    public void UpdateOrCreateIngredientTranslation(IngredientTranslation ingredientTranslation)
    {
        IngredientTranslation? existingTranslation = dbContext.IngredientTranslations.Find(ingredientTranslation.Id);
        if (existingTranslation == null)
        {
            dbContext.IngredientTranslations.Add(ingredientTranslation);
            dbContext.SaveChanges();
            return;
        }

        existingTranslation.Name = ingredientTranslation.Name;
        dbContext.SaveChanges();
    }

    public Task<List<IngredientTranslation>> GetIngredientTranslations(int ingredientId)
    {
        return dbContext.IngredientTranslations
            .Where(it => it.IngredientId == ingredientId)
            .ToListAsync();
    }

    public Task<Ingredient?> GetIngredientByName(string ingredientName, string localeValue)
    {
        if (localeValue == "en")
        {
            return dbContext.Ingredients.Include(i => i.Translations).Where(it => it.Name == ingredientName)
                .FirstOrDefaultAsync();
        }

        return dbContext.Ingredients
            .Include(i => i.Translations)
            .FirstOrDefaultAsync(i =>
                i.Translations.Any(it => it.Name == ingredientName && it.LanguageCode == localeValue));
    }
}