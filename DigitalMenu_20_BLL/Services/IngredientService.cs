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
}