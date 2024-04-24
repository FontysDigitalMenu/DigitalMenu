using DigitalMenu_20_BLL.Exceptions;
using DigitalMenu_20_BLL.Interfaces.Repositories;
using DigitalMenu_20_BLL.Interfaces.Services;
using DigitalMenu_20_BLL.Models;

namespace DigitalMenu_20_BLL.Services;

public class IngredientService(IIngredientRepository ingredientRepository) : IIngredientService
{
    public Task<Ingredient?> GetIngredientByNameAsync(string name)
    {
        return ingredientRepository.GetIngredientByNameAsync(name);
    }

    public async Task<List<Ingredient>> GetIngredients()
    {
        return await ingredientRepository.GetIngredients();
    }

    public async Task<bool> DeleteIngredientsByMenuItemId(int menuItemId)
    {
        if (menuItemId <= 0)
        {
            throw new NotFoundException("Menu item id not found.");
        }

        return await ingredientRepository.DeleteIngredientsByMenuItemId(menuItemId);
    }

    public async Task<Ingredient?> CreateIngredient(Ingredient ingredient)
    {
        if (string.IsNullOrEmpty(ingredient.Name))
        {
            throw new ArgumentException("Ingredient name cannot be null or empty.", nameof(ingredient.Name));
        }

        if (ingredient.Stock <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(ingredient.Stock), "Ingredient stock must be greater than 0.");
        }

        return await ingredientRepository.CreateIngredient(ingredient);
    }
}