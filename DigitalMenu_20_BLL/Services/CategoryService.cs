using DigitalMenu_20_BLL.Exceptions;
using DigitalMenu_20_BLL.Interfaces.Repositories;
using DigitalMenu_20_BLL.Interfaces.Services;
using DigitalMenu_20_BLL.Models;

namespace DigitalMenu_20_BLL.Services;

public class CategoryService(ICategoryRepository categoryRepository) : ICategoryService
{
    public async Task<List<Category>> GetCategories()
    {
        return await categoryRepository.GetCategories();
    }

    public async Task<Category?> GetCategoryByName(string categoryName)
    {
        if (string.IsNullOrEmpty(categoryName))
        {
            throw new NotFoundException("Category name is empty");
        }

        return await categoryRepository.GetCategoryByName(categoryName);
    }

    public async Task<Category> CreateCategory(string categoryName)
    {
        if (string.IsNullOrEmpty(categoryName))
        {
            throw new ArgumentException("Category name cannot be null or empty.", nameof(categoryName));
        }

        Category category = new()
        {
            Name = categoryName,
        };

        return await categoryRepository.CreateCategory(category);
    }
    
    public async Task<bool> DeleteCategoriesByMenuItemId(int menuItemId)
    {
        if (menuItemId <= 0)
        {
            throw new NotFoundException("Menu item id not found.");
        }
        
        return await categoryRepository.DeleteCategoriesByMenuItemId(menuItemId);
    }
}