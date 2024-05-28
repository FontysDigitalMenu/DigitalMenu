using DigitalMenu_20_BLL.Models;

namespace DigitalMenu_20_BLL.Interfaces.Services;

public interface IIngredientService
{
    Task<Ingredient?> GetIngredientByNameAsync(string name);

    Task<List<Ingredient>> GetIngredients();

    Task<List<Ingredient>> GetIngredientsPerPage(int currentPage, int amount);

    public int GetIngredientCount();

    public Task<bool> DeleteIngredientsByMenuItemId(int menuItemId);

    public Task<Ingredient?> CreateIngredient(Ingredient ingredient, string language);

    public Task<Ingredient?> GetIngredientById(int ingredientId);

    public Task<bool> UpdateIngredient(Ingredient ingredient, string language);

    public Task<bool> DeleteIngredient(int ingredientId);
}