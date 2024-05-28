using DigitalMenu_20_BLL.Exceptions;
using DigitalMenu_20_BLL.Interfaces.Repositories;
using DigitalMenu_20_BLL.Interfaces.Services;
using DigitalMenu_20_BLL.Models;

namespace DigitalMenu_20_BLL.Services;

public class MenuItemService(IMenuItemRepository menuItemRepository) : IMenuItemService
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

    public async Task<MenuItem?> CreateMenuItem(MenuItem menuItem)
    {
        return await menuItemRepository.CreateMenuItem(menuItem);
    }

    public async Task<MenuItem?> UpdateMenuItem(MenuItem menuItem)
    {
        /*MenuItem? originalMenuItem = menuItemRepository.GetMenuItemBy(menuItem.Id);

        if (originalMenuItem == null)
        {
            throw new NotFoundException("MenuItem does not exist");
        }

        originalMenuItem.Name = menuItem.Name;
        originalMenuItem.Description = menuItem.Description;
        originalMenuItem.Price = menuItem.Price;
        originalMenuItem.Ingredients = [];
        originalMenuItem.Categories = null;

        if (string.IsNullOrEmpty(menuItem.ImageUrl))
        {
            menuItem.ImageUrl = originalMenuItem.ImageUrl;
        }

        originalMenuItem.ImageUrl = menuItem.ImageUrl;*/

        return await menuItemRepository.UpdateMenuItem(menuItem);
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
}