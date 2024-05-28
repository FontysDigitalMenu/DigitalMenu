using DigitalMenu_20_BLL.Exceptions;
using DigitalMenu_20_BLL.Interfaces.Repositories;
using DigitalMenu_20_BLL.Interfaces.Services;
using DigitalMenu_20_BLL.Models;

namespace DigitalMenu_20_BLL.Services;

public class IngredientService(IIngredientRepository ingredientRepository, ITranslationService translationService)
    : IIngredientService
{
    private readonly List<string> _languages = ["en", "nl", "de", "ko"];

    public Task<Ingredient?> GetIngredientByNameAsync(string name)
    {
        return ingredientRepository.GetIngredientByNameAsync(name);
    }

    public async Task<List<Ingredient>> GetIngredients()
    {
        return await ingredientRepository.GetIngredients();
    }

    public async Task<List<Ingredient>> GetIngredientsPerPage(int currentPage, int amount)
    {
        int lastIngredient = (currentPage - 1) * amount;
        return await ingredientRepository.GetIngredientsPerPage(lastIngredient, amount);
    }

    public int GetIngredientCount()
    {
        return ingredientRepository.GetIngredientCount();
    }

    public async Task<bool> DeleteIngredientsByMenuItemId(int menuItemId)
    {
        if (menuItemId <= 0)
        {
            throw new NotFoundException("Menu item id not found.");
        }

        return await ingredientRepository.DeleteIngredientsByMenuItemId(menuItemId);
    }

    public async Task<Ingredient?> CreateIngredient(Ingredient ingredient, string language)
    {
        if (string.IsNullOrEmpty(ingredient.Name))
        {
            throw new ArgumentException("Ingredient name cannot be null or empty.", nameof(ingredient.Name));
        }

        if (ingredient.Stock <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(ingredient.Stock), "Ingredient stock must be greater than 0.");
        }

        List<IngredientTranslation> ingredientTranslations = await GetIngredientTranslations(ingredient, language);
        IngredientTranslation englishIngredient = ingredientTranslations.First(it => it.LanguageCode == "en");
        ingredient.Name = englishIngredient.Name;
        ingredientTranslations.Remove(englishIngredient);

        Ingredient? createdIngredient = await ingredientRepository.CreateIngredient(ingredient);
        if (createdIngredient == null)
        {
            return null;
        }

        ingredientTranslations.ForEach(it => it.IngredientId = createdIngredient.Id);
        ingredientRepository.CreateIngredientTranslations(ingredientTranslations);

        return createdIngredient;
    }

    public async Task<Ingredient?> GetIngredientById(int ingredientId)
    {
        if (ingredientId <= 0)
        {
            throw new NotFoundException("Ingredient id not found.");
        }

        return await ingredientRepository.GetIngredientById(ingredientId);
    }

    public async Task<bool> UpdateIngredient(Ingredient ingredient, string language)
    {
        if (string.IsNullOrEmpty(ingredient.Name))
        {
            throw new ArgumentException("Ingredient name cannot be null or empty.", nameof(ingredient.Name));
        }

        if (ingredient.Stock <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(ingredient.Stock), "Ingredient stock must be greater than 0.");
        }

        List<IngredientTranslation> ingredientTranslations = await GetIngredientTranslations(ingredient, language);
        IngredientTranslation englishIngredient = ingredientTranslations.First(it => it.LanguageCode == "en");
        ingredient.Name = englishIngredient.Name;
        ingredientTranslations.Remove(englishIngredient);

        bool updateIsSuccess = await ingredientRepository.UpdateIngredient(ingredient);
        if (!updateIsSuccess)
        {
            return false;
        }

        foreach (IngredientTranslation ingredientTranslation in ingredientTranslations)
        {
            ingredientRepository.UpdateOrCreateIngredientTranslation(ingredientTranslation);
        }

        return updateIsSuccess;
    }

    public async Task<bool> DeleteIngredient(int ingredientId)
    {
        if (ingredientId <= 0)
        {
            throw new NotFoundException("Ingredient id not found.");
        }

        return await ingredientRepository.DeleteIngredient(ingredientId);
    }

    private async Task<List<IngredientTranslation>> GetIngredientTranslations(Ingredient ingredient, string language)
    {
        List<IngredientTranslation> ingredientTranslations =
        [
            new IngredientTranslation
            {
                IngredientId = ingredient.Id,
                LanguageCode = language,
                Name = ingredient.Name,
            },
        ];
        foreach (string lang in _languages.Where(l => l != language))
        {
            string translatedName = await translationService.Translate(ingredient.Name, language, lang);

            ingredientTranslations.Add(new IngredientTranslation
            {
                IngredientId = ingredient.Id,
                LanguageCode = lang,
                Name = translatedName,
            });
        }

        List<IngredientTranslation> existingTranslations =
            await ingredientRepository.GetIngredientTranslations(ingredient.Id);
        foreach (IngredientTranslation existingTranslation in existingTranslations)
        {
            IngredientTranslation? translation =
                ingredientTranslations.FirstOrDefault(it => it.LanguageCode == existingTranslation.LanguageCode);
            if (translation != null)
            {
                translation.Id = existingTranslation.Id;
            }
        }

        return ingredientTranslations;
    }
}