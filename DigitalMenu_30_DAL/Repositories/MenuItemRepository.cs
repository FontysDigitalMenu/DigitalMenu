using DigitalMenu_20_BLL.Interfaces.Repositories;
using DigitalMenu_20_BLL.Models;
using DigitalMenu_30_DAL.Data;
using Microsoft.EntityFrameworkCore;

namespace DigitalMenu_30_DAL.Repositories;

public class MenuItemRepository(ApplicationDbContext dbContext) : IMenuItemRepository
{
    public IEnumerable<MenuItem> GetNextMenuItems(int lastId, int amount)
    {
        return dbContext.MenuItems
            .OrderBy(m => m.Id)
            .Where(m => m.Id > lastId)
            .Take(amount)
            .ToList();
    }
    
    public IEnumerable<MenuItem> GetNextMenuItemsWithCategory(int lastId, int amount)
    {
        return dbContext.MenuItems
            .Include(m => m.Categories)
            .OrderBy(m => m.Id)
            .Where(m => m.Id > lastId)
            .Take(amount)
            .ToList();
    }
    
    public IEnumerable<Category> GetCategories()
    {
        return dbContext.Categories
            .ToList();
    }
    
    public MenuItem? GetMenuItemBy(int id)
    {
        var menuItemWithIngredients = dbContext.MenuItemIngredients
            .Where(mii => mii.MenuItemId == id)
            .Include(mii => mii.Ingredient)
            .Select(mii => new
            {
                mii.MenuItem, mii.Ingredient,
            })
            .ToList();
        
        if (menuItemWithIngredients.Any())
        {
            MenuItem firstMenuItem = menuItemWithIngredients.First().MenuItem;
            MenuItem menuItem = new()
            {
                Id = firstMenuItem.Id,
                Name = firstMenuItem.Name,
                Description = firstMenuItem.Description,
                ImageUrl = firstMenuItem.ImageUrl,
                Price = firstMenuItem.Price,
                Ingredients = menuItemWithIngredients.Select(m => m.Ingredient).ToList(),
            };
            
            return menuItem;
        }
        
        return dbContext.MenuItems.Find(id);
    }
}