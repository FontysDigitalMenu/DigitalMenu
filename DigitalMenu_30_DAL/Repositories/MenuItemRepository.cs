using DigitalMenu_20_BLL.Exceptions;
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
            .Include(m => m.Translations)
            .OrderBy(m => m.Id)
            .Where(m => m.IsActive)
            .Where(m => m.Id > lastId)
            .Take(amount)
            .ToList();
    }

    public IEnumerable<MenuItem> GetNextMenuItemsWithCategory(int lastId, int amount)
    {
        List<MenuItem> menuItems = dbContext.CategoryMenuItems
            .OrderBy(cm => cm.CategoryId)
            .Skip(lastId)
            .Take(amount)
            .Include(cm => cm.MenuItem)
            .ThenInclude(mi => mi.Translations)
            .Include(cm => cm.Category)
            .ThenInclude(c => c.Translations)
            .Where(cm => cm.MenuItem.IsActive)
            .ToList()
            .Select(cm => new MenuItem
            {
                Id = cm.MenuItem.Id,
                Name = cm.MenuItem.Name,
                Description = cm.MenuItem.Description,
                Price = cm.MenuItem.Price,
                ImageUrl = cm.MenuItem.ImageUrl,
                Translations = cm.MenuItem.Translations,
                Categories = [cm.Category],
                Ingredients = dbContext.MenuItemIngredients
                    .Where(mii => mii.MenuItemId == cm.MenuItem.Id)
                    .Select(mii => new Ingredient
                    {
                        Id = mii.Ingredient.Id,
                        Name = mii.Ingredient.Name,
                        Pieces = mii.Pieces,
                        Stock = mii.Ingredient.Stock,
                    }).ToList(),
            })
            .ToList();

        return menuItems;
    }

    public IEnumerable<Category> GetCategories()
    {
        return dbContext.Categories
            .Include(c => c.Translations)
            .ToList();
    }

    public MenuItem? GetMenuItemBy(int id)
    {
        var menuItemWithIngredients = dbContext.MenuItemIngredients
            .Where(mii => mii.MenuItemId == id)
            .Include(mii => mii.Ingredient)
            .ThenInclude(i => i.Translations)
            .Include(mii => mii.MenuItem)
            .ThenInclude(mi => mi.Translations)
            .Select(mii => new
            {
                mii.MenuItem, mii.Ingredient, mii.Pieces,
            })
            .ToList();

        List<Category> categories = dbContext.CategoryMenuItems
            .Where(mc => mc.MenuItemId == id)
            .Include(mc => mc.Category)
            .ThenInclude(c => c.Translations)
            .Select(mc => mc.Category)
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
                Ingredients = menuItemWithIngredients.Select(m => new Ingredient
                {
                    Id = m.Ingredient.Id,
                    Name = m.Ingredient.Name,
                    Pieces = m.Pieces,
                    Translations = m.Ingredient.Translations,
                    Stock = m.Ingredient.Stock,
                }).ToList(),
                Categories = categories,
                Translations = firstMenuItem.Translations,
            };

            return menuItem;
        }

        MenuItem? menuItemWithoutIngredient =
            dbContext.MenuItems.Include(mi => mi.Translations).FirstOrDefault(mi => mi.Id == id);

        if (menuItemWithoutIngredient != null)
        {
            menuItemWithoutIngredient.Categories = categories;
        }

        return menuItemWithoutIngredient;
    }

    public async Task<List<MenuItem>> GetMenuItems(int lastMenuItem, int amount)
    {
        return await dbContext.MenuItems
            .Include(m => m.Translations)
            .OrderBy(m => m.Id)
            .Skip(lastMenuItem)
            .Take(amount)
            .Where(m => m.IsActive)
            .ToListAsync();
    }

    public int GetMenuItemCount()
    {
        return dbContext.MenuItems
            .Count();
    }

    public async Task<MenuItem?> CreateMenuItem(MenuItem menuItem)
    {
        dbContext.MenuItems.Add(menuItem);
        return await dbContext.SaveChangesAsync() > 0 ? menuItem : null;
    }

    public async Task<MenuItem?> UpdateMenuItem(MenuItem menuItem)
    {
        MenuItem? existingMenuItem = await dbContext.MenuItems.FindAsync(menuItem.Id);

        if (existingMenuItem == null)
        {
            throw new NotFoundException("MenuItem does not exist");
        }

        if (existingMenuItem.Name == menuItem.Name &&
            existingMenuItem.Description == menuItem.Description &&
            existingMenuItem.Price == menuItem.Price &&
            existingMenuItem.ImageUrl == menuItem.ImageUrl)
        {
            return menuItem;
        }

        existingMenuItem.Name = menuItem.Name;
        existingMenuItem.Description = menuItem.Description;
        existingMenuItem.Price = menuItem.Price;
        existingMenuItem.ImageUrl =
            string.IsNullOrEmpty(menuItem.ImageUrl) ? existingMenuItem.ImageUrl : menuItem.ImageUrl;

        await dbContext.SaveChangesAsync();
        return menuItem;
    }

    public async Task<List<MenuItemIngredient>?> AddIngredientsToMenuItem(List<MenuItemIngredient> menuItemIngredients)
    {
        await dbContext.MenuItemIngredients.AddRangeAsync(menuItemIngredients);
        return await dbContext.SaveChangesAsync() > 0 ? menuItemIngredients : null;
    }

    public async Task<List<CategoryMenuItem>?> AddCategoriesToMenuItem(List<CategoryMenuItem> categoryMenuItems)
    {
        await dbContext.CategoryMenuItems.AddRangeAsync(categoryMenuItems);
        return await dbContext.SaveChangesAsync() > 0 ? categoryMenuItems : null;
    }

    public bool Delete(int id)
    {
        MenuItem entityToDelete = dbContext.MenuItems.Find(id)!;
        entityToDelete.IsActive = false;
        dbContext.SaveChanges();
        return true;
    }

    public void CreateMenuItemTranslations(List<MenuItemTranslation> menuItemTranslations)
    {
        dbContext.MenuItemTranslations.AddRange(menuItemTranslations);
        dbContext.SaveChanges();
    }

    public void UpdateOrCreateMenuItemTranslation(MenuItemTranslation menuItemTranslation)
    {
        MenuItemTranslation? existingTranslation = dbContext.MenuItemTranslations.Find(menuItemTranslation.Id);
        if (existingTranslation == null)
        {
            dbContext.MenuItemTranslations.Add(menuItemTranslation);
            dbContext.SaveChanges();
            return;
        }

        existingTranslation.Name = menuItemTranslation.Name;
        existingTranslation.Description = menuItemTranslation.Description;
        dbContext.SaveChanges();
    }

    public List<MenuItemTranslation> GetMenuItemTranslations(int menuItemId)
    {
        return dbContext.MenuItemTranslations
            .Where(mt => mt.MenuItemId == menuItemId)
            .ToList();
    }
}