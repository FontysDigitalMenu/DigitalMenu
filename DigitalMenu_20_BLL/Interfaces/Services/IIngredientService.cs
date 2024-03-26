using DigitalMenu_20_BLL.Models;

namespace DigitalMenu_20_BLL.Interfaces.Services;

public interface IIngredientService
{
    Task<Ingredient?> GetIngredientByNameAsync(string name);
}