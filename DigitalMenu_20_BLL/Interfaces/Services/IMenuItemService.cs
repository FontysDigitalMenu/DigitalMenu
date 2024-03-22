using DigitalMenu_20_BLL.Models;

namespace DigitalMenu_20_BLL.Interfaces.Services;

public interface IMenuItemService
{
    public IEnumerable<MenuItem> GetNextMenuItems(int lastId, int amount);

    public IEnumerable<Category> GetCategoriesWithNextMenuItems(int lastId, int amount);

    public MenuItem? GetMenuItemById(int id);
}