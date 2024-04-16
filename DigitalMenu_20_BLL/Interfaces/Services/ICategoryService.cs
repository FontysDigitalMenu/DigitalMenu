using DigitalMenu_20_BLL.Models;

namespace DigitalMenu_20_BLL.Interfaces.Services;

public interface ICategoryService
{
    public Task<List<Category>> GetCategories();
}