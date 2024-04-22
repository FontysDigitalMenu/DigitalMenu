﻿using DigitalMenu_20_BLL.Interfaces.Repositories;
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
            .Where(m => m.IsActive)
            .Where(m => m.Id > lastId)
            .Take(amount)
            .ToList();
    }

    public IEnumerable<MenuItem> GetNextMenuItemsWithCategory(int lastId, int amount)
    {
        IEnumerable<MenuItem> menuItems = dbContext.CategoryMenuItems
            .OrderBy(cm => cm.CategoryId)
            .Skip(lastId)
            .Take(amount)
            .Include(cm => cm.MenuItem)
            .Include(cm => cm.Category)
            .Where(cm => cm.MenuItem.IsActive)
            .Select(cm => new MenuItem
            {
                Id = cm.MenuItem.Id,
                Name = cm.MenuItem.Name,
                Description = cm.MenuItem.Description,
                Price = cm.MenuItem.Price,
                ImageUrl = cm.MenuItem.ImageUrl,
                Categories = new List<Category> { cm.Category },
            })
            .ToList();

        return menuItems;
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

    public async Task<List<MenuItem>> GetMenuItems()
    {
        return await dbContext.MenuItems
            .Include(m => m.Categories)
            .OrderBy(m => m.Id)
            .Where(m => m.IsActive)
            .ToListAsync();
    }

    public async Task<MenuItem?> CreateMenuItem(MenuItem menuItem)
    {
        dbContext.MenuItems.Add(menuItem);
        return dbContext.SaveChanges() > 0 ? menuItem : null;
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
}