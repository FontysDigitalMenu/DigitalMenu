using DigitalMenu_20_BLL.Models;

namespace DigitalMenu_20_BLL.Interfaces.Services;

public interface IMenuItemService
{
    public IEnumerable<MenuItem> GetNextMenuItems(int lastId, int amount);

    public IEnumerable<Category> GetCategoriesWithNextMenuItems(int lastId, int amount);

    public MenuItem? GetMenuItemById(int id);

    public Task<List<MenuItem>> GetMenuItems(int currentPage, int amount);

    public int GetMenuItemCount();

    public Task<MenuItem?> CreateMenuItem(MenuItem menuItem, string language);

    public Task<MenuItem?> UpdateMenuItem(MenuItem menuItem, string language);

    public Task<List<MenuItemIngredient>?> AddIngredientsToMenuItem(List<MenuItemIngredient> menuItemIngredients);

    public Task<List<CategoryMenuItem>?> AddCategoriesToMenuItem(List<Category> categories, int menuItemId);

    public bool Delete(int id);
}