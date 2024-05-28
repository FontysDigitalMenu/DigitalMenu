using DigitalMenu_20_BLL.Models;

namespace DigitalMenu_20_BLL.Interfaces.Repositories;

public interface IMenuItemRepository
{
    public IEnumerable<MenuItem> GetNextMenuItems(int lastId, int amount);

    public IEnumerable<MenuItem> GetNextMenuItemsWithCategory(int lastId, int amount);

    public IEnumerable<Category> GetCategories();

    public MenuItem? GetMenuItemBy(int id);

    public Task<List<MenuItem>> GetMenuItems(int lastMenuItem, int amount);

    public int GetMenuItemCount();

    public Task<MenuItem?> CreateMenuItem(MenuItem menuItem);

    public Task<MenuItem?> UpdateMenuItem(MenuItem menuItem);

    public Task<List<MenuItemIngredient>?> AddIngredientsToMenuItem(List<MenuItemIngredient> menuItemIngredients);

    public Task<List<CategoryMenuItem>?> AddCategoriesToMenuItem(List<CategoryMenuItem> categoryMenuItems);

    public bool Delete(int id);

    public void CreateMenuItemTranslations(List<MenuItemTranslation> menuItemTranslations);

    public void UpdateOrCreateMenuItemTranslation(MenuItemTranslation menuItemTranslation);

    public List<MenuItemTranslation> GetMenuItemTranslations(int menuItemId);
}