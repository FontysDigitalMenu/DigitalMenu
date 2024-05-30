using DigitalMenu_20_BLL.Exceptions;
using DigitalMenu_20_BLL.Interfaces.Repositories;
using DigitalMenu_20_BLL.Interfaces.Services;
using DigitalMenu_20_BLL.Models;

namespace DigitalMenu_20_BLL.Services;

public class CategoryService(ICategoryRepository categoryRepository, ITranslationService translationService)
    : ICategoryService
{
    public async Task<List<Category>> GetCategories()
    {
        return await categoryRepository.GetCategories();
    }

    public async Task<Category?> GetCategoryByName(string categoryName)
    {
        if (string.IsNullOrEmpty(categoryName))
        {
            throw new NotFoundException("Category name is empty");
        }

        return await categoryRepository.GetCategoryByName(categoryName);
    }

    public async Task<Category?> GetCategoryByName(string categoryName, string locale)
    {
        return await categoryRepository.GetCategoryByName(categoryName, locale);
    }

    public async Task<Category> CreateCategory(string categoryName, string language)
    {
        if (string.IsNullOrEmpty(categoryName))
        {
            throw new ArgumentException("Category name cannot be null or empty.", nameof(categoryName));
        }

        List<CategoryTranslation> categoryTranslations = await GenerateCategoryTranslations(categoryName, language);
        CategoryTranslation englishCategory = categoryTranslations.First(ct => ct.LanguageCode == "en");

        Category category = new()
        {
            Name = englishCategory.Name,
        };
        Category createdCategory = await categoryRepository.CreateCategory(category);

        categoryTranslations.Remove(englishCategory);
        categoryTranslations.ForEach(ct => ct.CategoryId = createdCategory.Id);
        categoryRepository.CreateTranslations(categoryTranslations);

        return category;
    }

    public async Task<bool> DeleteCategoriesByMenuItemId(int menuItemId)
    {
        if (menuItemId <= 0)
        {
            throw new NotFoundException("Menu item id not found.");
        }

        return await categoryRepository.DeleteCategoriesByMenuItemId(menuItemId);
    }

    private async Task<List<CategoryTranslation>> GenerateCategoryTranslations(string categoryName, string language)
    {
        List<CategoryTranslation> categoryTranslations =
        [
            new CategoryTranslation { Name = categoryName, LanguageCode = language },
        ];

        foreach (string lang in TranslationService.SupportedLanguages.Where(l => l != language))
        {
            string translatedName = await translationService.Translate(categoryName, language, lang);

            categoryTranslations.Add(new CategoryTranslation
            {
                LanguageCode = lang,
                Name = translatedName,
            });
        }

        return categoryTranslations;
    }
}