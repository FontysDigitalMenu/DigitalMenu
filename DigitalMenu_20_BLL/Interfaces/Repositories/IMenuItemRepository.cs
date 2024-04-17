﻿using DigitalMenu_20_BLL.Models;

namespace DigitalMenu_20_BLL.Interfaces.Repositories;

public interface IMenuItemRepository
{
    public IEnumerable<MenuItem> GetNextMenuItems(int lastId, int amount);

    public IEnumerable<MenuItem> GetNextMenuItemsWithCategory(int lastId, int amount);

    public IEnumerable<Category> GetCategories();

    public MenuItem? GetMenuItemBy(int id);

    public Task<List<MenuItem>> GetMenuItems();
    public Task<MenuItem?> CreateMenuItem(MenuItem menuItem);

    public Task<List<MenuItemIngredient>?> AddIngredientsToMenuItem(List<MenuItemIngredient> menuItemIngredients);
}