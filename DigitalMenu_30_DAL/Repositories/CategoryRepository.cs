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
            .OrderBy(c => c.Id)
            .ToListAsync();
    }

    public async Task<Category?> GetCategoryByName(string categoryName)
    {
        return await dbContext.Categories.FirstOrDefaultAsync(c => c.Name == categoryName);
    }

    public async Task<Category> CreateCategory(Category category)
    {
        await dbContext.Categories.AddAsync(category);
        await dbContext.SaveChangesAsync();

        return category;
    }
}