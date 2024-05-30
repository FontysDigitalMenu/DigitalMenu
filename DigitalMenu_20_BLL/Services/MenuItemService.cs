using DigitalMenu_20_BLL.Exceptions;
using DigitalMenu_20_BLL.Interfaces.Repositories;
using DigitalMenu_20_BLL.Interfaces.Services;
using DigitalMenu_20_BLL.Models;

namespace DigitalMenu_20_BLL.Services;

public class MenuItemService(IMenuItemRepository menuItemRepository, ITranslationService translationService)
    : IMenuItemService
{
    public IEnumerable<Category> GetCategoriesWithNextMenuItems(int lastId, int amount)
    {
        IEnumerable<MenuItem> menuItems = menuItemRepository.GetNextMenuItemsWithCategory(lastId, amount);
        IEnumerable<Category> categories = menuItemRepository.GetCategories();

        List<Category> categoriesWithMenuItems = categories
            .Select(c => new Category
            {
                Id = c.Id,
                Name = c.Name,
                Translations = c.Translations,
                MenuItems = menuItems.Where(mi => mi.Categories.Any(mc => mc.Name == c.Name)).ToList(),
            })
            .Where(c => c.MenuItems.Count != 0)
            .ToList();

        return categoriesWithMenuItems;
    }

    public IEnumerable<MenuItem> GetNextMenuItems(int lastId, int amount)
    {
        return menuItemRepository.GetNextMenuItems(lastId, amount);
    }

    public MenuItem? GetMenuItemById(int id)
    {
        return menuItemRepository.GetMenuItemBy(id);
    }

    public async Task<List<MenuItem>> GetMenuItems(int currentPage, int amount)
    {
        int lastMenuItem = (currentPage - 1) * amount;
        return await menuItemRepository.GetMenuItems(lastMenuItem, amount);
    }

    public int GetMenuItemCount()
    {
        return menuItemRepository.GetMenuItemCount();
    }

    public async Task<MenuItem?> CreateMenuItem(MenuItem menuItem, string language)
    {
        List<MenuItemTranslation> menuItemTranslations = await GetMenuItemTranslations(menuItem, language);
        MenuItemTranslation englishMenuItem = menuItemTranslations.First(mit => mit.LanguageCode == "en");
        menuItem.Name = englishMenuItem.Name;
        menuItem.Description = englishMenuItem.Description;
        menuItemTranslations.Remove(englishMenuItem);

        MenuItem? createdMenuItem = await menuItemRepository.CreateMenuItem(menuItem);
        menuItemTranslations.ForEach(mit => mit.MenuItemId = createdMenuItem.Id);
        menuItemRepository.CreateMenuItemTranslations(menuItemTranslations);

        return createdMenuItem;
    }

    public async Task<MenuItem?> UpdateMenuItem(MenuItem menuItem, string language)
    {
        List<MenuItemTranslation> menuItemTranslations = await GetMenuItemTranslations(menuItem, language);
        MenuItemTranslation englishMenuItem = menuItemTranslations.First(mit => mit.LanguageCode == "en");
        menuItem.Name = englishMenuItem.Name;
        menuItem.Description = englishMenuItem.Description;
        menuItemTranslations.Remove(englishMenuItem);

        MenuItem? updatedMenuItem = await menuItemRepository.UpdateMenuItem(menuItem);
        foreach (MenuItemTranslation menuItemTranslation in menuItemTranslations)
        {
            menuItemRepository.UpdateOrCreateMenuItemTranslation(menuItemTranslation);
        }

        return updatedMenuItem;
    }

    public async Task<List<MenuItemIngredient>?> AddIngredientsToMenuItem(List<MenuItemIngredient> menuItemIngredients)
    {
        return await menuItemRepository.AddIngredientsToMenuItem(menuItemIngredients);
    }

    public async Task<List<CategoryMenuItem>?> AddCategoriesToMenuItem(List<Category> categories, int menuItemId)
    {
        List<CategoryMenuItem> menuItemCategories = categories
            .Select(category => new CategoryMenuItem
            {
                CategoryId = category.Id,
                MenuItemId = menuItemId,
            })
            .ToList();

        return await menuItemRepository.AddCategoriesToMenuItem(menuItemCategories);
    }

    public bool Delete(int id)
    {
        MenuItem? existingMenuItem = GetMenuItemById(id);
        if (existingMenuItem == null || !existingMenuItem.IsActive)
        {
            throw new NotFoundException("MenuItem does not exist");
        }

        return menuItemRepository.Delete(id);
    }

    private async Task<List<MenuItemTranslation>> GetMenuItemTranslations(MenuItem menuItem, string language)
    {
        List<MenuItemTranslation> menuItemTranslations =
        [
            new MenuItemTranslation
            {
                MenuItemId = menuItem.Id,
                LanguageCode = language,
                Name = menuItem.Name,
                Description = menuItem.Description,
            },
        ];
        foreach (string lang in TranslationService.SupportedLanguages.Where(l => l != language))
        {
            string translatedName = await translationService.Translate(menuItem.Name, language, lang);
            string translatedDescription = await translationService.Translate(menuItem.Description, language, lang);

            menuItemTranslations.Add(new MenuItemTranslation
            {
                MenuItemId = menuItem.Id,
                LanguageCode = lang,
                Name = translatedName,
                Description = translatedDescription,
            });
        }

        List<MenuItemTranslation> existingTranslations = menuItemRepository.GetMenuItemTranslations(menuItem.Id);
        foreach (MenuItemTranslation existingTranslation in existingTranslations)
        {
            MenuItemTranslation? translation =
                menuItemTranslations.FirstOrDefault(mt => mt.LanguageCode == existingTranslation.LanguageCode);
            if (translation != null)
            {
                translation.Id = existingTranslation.Id;
            }
        }

        return menuItemTranslations;
    }
}