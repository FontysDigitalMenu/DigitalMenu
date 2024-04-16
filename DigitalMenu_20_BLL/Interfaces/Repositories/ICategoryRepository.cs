using DigitalMenu_20_BLL.Models;

namespace DigitalMenu_20_BLL.Interfaces.Repositories;

public interface ICategoryRepository
{
    public Task<List<Category>> GetCategories();
}