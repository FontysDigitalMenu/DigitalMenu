using DigitalMenu_20_BLL.Interfaces.Repositories;
using DigitalMenu_20_BLL.Models;
using DigitalMenu_30_DAL.Data;
using Microsoft.EntityFrameworkCore;

namespace DigitalMenu_30_DAL.Repositories;

public class CategoryRepository(ApplicationDbContext dbContext) : ICategoryRepository
{
    public async Task<List<Category>> GetCategories()
    {
        return await dbContext.Categories
            .Include(c => c.Translations)
            .OrderBy(c => c.Id)
            .ToListAsync();
    }

    public async Task<Category?> GetCategoryByName(string categoryName)
    {
        return await dbContext.Categories.FirstOrDefaultAsync(c => c.Name == categoryName);
    }

    public async Task<Category?> GetCategoryByName(string categoryName, string locale)
    {
        CategoryTranslation? categoryTranslation = await dbContext.CategoryTranslations
            .Include(ct => ct.Category)
            .FirstOrDefaultAsync(ct => ct.Name == categoryName && ct.LanguageCode == locale);

        return categoryTranslation?.Category;
    }

    public void CreateTranslations(List<CategoryTranslation> categoryTranslations)
    {
        dbContext.CategoryTranslations.AddRange(categoryTranslations);
        dbContext.SaveChanges();
    }

    public async Task<Category> CreateCategory(Category category)
    {
        await dbContext.Categories.AddAsync(category);
        await dbContext.SaveChangesAsync();

        return category;
    }

    public async Task<bool> DeleteCategoriesByMenuItemId(int menuItemId)
    {
        List<CategoryMenuItem> categoryMenuItems = await dbContext.CategoryMenuItems
            .Where(cmi => cmi.MenuItemId == menuItemId)
            .ToListAsync();

        if (categoryMenuItems.Count == 0)
        {
            return true;
        }

        dbContext.CategoryMenuItems.RemoveRange(categoryMenuItems);
        await dbContext.SaveChangesAsync();

        return true;
    }
}