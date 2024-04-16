using DigitalMenu_20_BLL.Models;

namespace DigitalMenu_20_BLL.Interfaces.Repositories;

public interface IMenuItemRepository
{
    public IEnumerable<MenuItem> GetNextMenuItems(int lastId, int amount);

    public IEnumerable<MenuItem> GetNextMenuItemsWithCategory(int lastId, int amount);

    public IEnumerable<Category> GetCategories();

    public MenuItem? GetMenuItemBy(int id);
    
    public bool Delete(int id);
}