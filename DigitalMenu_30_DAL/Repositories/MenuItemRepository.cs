using DigitalMenu_20_BLL.Interfaces.Repositories;
using DigitalMenu_20_BLL.Models;
using DigitalMenu_30_DAL.Data;
using Microsoft.EntityFrameworkCore;

namespace DigitalMenu_30_DAL.Repositories;

public class MenuItemRepository : IMenuItemRepository
{
    private readonly ApplicationDbContext _dbContext;

    public MenuItemRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public IEnumerable<MenuItem> GetNextMenuItems(int lastId, int amount)
    {
        return _dbContext.MenuItems
            .OrderBy(m => m.Id)
            .Where(m => m.Id > lastId)
            .Take(amount)
            .ToList();
    }

    public IEnumerable<MenuItem> GetNextMenuItemsWithCategory(int lastId, int amount)
    {
        return _dbContext.MenuItems
            .Include(m => m.Categories)
            .OrderBy(m => m.Id)
            .Where(m => m.Id > lastId)
            .Take(amount)
            .ToList();
    }

    public IEnumerable<Category> GetCategories()
    {
        return _dbContext.Categories
            .ToList();
    }

    public MenuItem? GetMenuItemBy(int id)
    {
        var menuItemWithIngredients = _dbContext.MenuItemIngredients
            .Where(mii => mii.MenuItemId == id)
            .Include(mii => mii.Ingredient)
            .Select(mii => new
            {
                MenuItem = mii.MenuItem,
                Ingredient = mii.Ingredient
            })
            .ToList();

        if (menuItemWithIngredients.Any())
        {
            var firstMenuItem = menuItemWithIngredients.First().MenuItem;
            var menuItem = new MenuItem
            {
                Id = firstMenuItem.Id,
                Name = firstMenuItem.Name,
                Description = firstMenuItem.Description,
                ImageUrl = firstMenuItem.ImageUrl,
                Price = firstMenuItem.Price,
                Ingredients = menuItemWithIngredients.Select(m => m.Ingredient).ToList()
            };

            return menuItem;
        }
        else
        {
            return _dbContext.MenuItems.Find(id);
        }
    }
}