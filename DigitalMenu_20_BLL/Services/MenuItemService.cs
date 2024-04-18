﻿using DigitalMenu_20_BLL.Interfaces.Repositories;
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

    public async Task<List<MenuItem>> GetMenuItems()
    {
        return await menuItemRepository.GetMenuItems();
    }

    public async Task<MenuItem?> CreateMenuItem(MenuItem menuItem)
    {
        return await menuItemRepository.CreateMenuItem(menuItem);
    }

    public async Task<List<MenuItemIngredient>?> AddIngredientsToMenuItem(List<MenuItemIngredient> menuItemIngredients)
    {
        return await menuItemRepository.AddIngredientsToMenuItem(menuItemIngredients);
    }
}