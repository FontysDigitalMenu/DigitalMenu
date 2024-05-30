using DigitalMenu_20_BLL.Models;

namespace DigitalMenu_20_BLL.Interfaces.Repositories;

public interface IIngredientRepository
{
    Task<Ingredient?> GetIngredientByNameAsync(string name);

    Task<List<Ingredient>> GetIngredients();

    Task<List<Ingredient>> GetIngredientsPerPage(int lastIngredient, int amount);

    public int GetIngredientCount();

    public Task<bool> DeleteIngredientsByMenuItemId(int menuItemId);

    public Task<Ingredient?> CreateIngredient(Ingredient ingredient);

    public Task<Ingredient?> GetIngredientById(int ingredientId);

    public Task<bool> UpdateIngredient(Ingredient ingredient);

    public Task<bool> DeleteIngredient(int ingredientId);

    public void CreateIngredientTranslations(List<IngredientTranslation> ingredientTranslations);

    public void UpdateOrCreateIngredientTranslation(IngredientTranslation ingredientTranslation);

    public Task<List<IngredientTranslation>> GetIngredientTranslations(int ingredientId);

    public Task<Ingredient?> GetIngredientByName(string ingredientName, string localeValue);
}