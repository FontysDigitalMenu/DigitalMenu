using DigitalMenu_20_BLL.Models;

namespace DigitalMenu_20_BLL.Interfaces.Repositories;

public interface IIngredientRepository
{
    Task<Ingredient?> GetIngredientByNameAsync(string name);

    Task<List<Ingredient>> GetIngredients();
}