using DigitalMenu_20_BLL.Models;

namespace DigitalMenu_20_BLL.Interfaces.Services;

public interface ICategoryService
{
    public Task<List<Category>> GetCategories();

    public Task<Category?> GetCategoryByName(string categoryName);

    public Task<Category> CreateCategory(string categoryName);

    public Task<bool> DeleteCategoriesByMenuItemId(int menuItemId);
}