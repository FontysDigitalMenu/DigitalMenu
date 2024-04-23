using DigitalMenu_20_BLL.Models;

namespace DigitalMenu_20_BLL.Interfaces.Repositories;

public interface ICategoryRepository
{
    public Task<List<Category>> GetCategories();

    public Task<Category?> GetCategoryByName(string categoryName);

    public Task<Category> CreateCategory(Category category);
    public Task<bool> DeleteCategoriesByMenuItemId(int menuItemId);
}