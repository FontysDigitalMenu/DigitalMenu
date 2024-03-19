using DigitalMenu_20_BLL.Interfaces.Repositories;
using DigitalMenu_20_BLL.Interfaces.Services;
using DigitalMenu_20_BLL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalMenu_20_BLL.Services
{
    public class MenuItemService : IMenuItemService
    {
        private readonly IMenuItemRepository _menuItemRepository;
        public MenuItemService(IMenuItemRepository menuItemRepository)
        {
            _menuItemRepository = menuItemRepository;
        }

        public IEnumerable<Category> GetCategoriesWithNextMenuItems(int lastId, int amount)
        {
            var menuItems = _menuItemRepository.GetNextMenuItemsWithCategory(lastId, amount);
            var categories = _menuItemRepository.GetCategories();

            var categoriesWithMenuItems = categories
                .Select(c => new Category
                {
                    Id = c.Id,
                    Name = c.Name,
                    MenuItems = menuItems.Where(mi => mi.Categories.Any(mc => mc.Name == c.Name)).ToList()
                })
                .Where(c => c.MenuItems.Count != 0)
                .ToList();

            return categoriesWithMenuItems;
        }

        public IEnumerable<MenuItem> GetNextMenuItems(int lastId, int amount)
        {
            return _menuItemRepository.GetNextMenuItems(lastId, amount);
        }

        public MenuItem GetMenuItemBy(int id)
        {
            return _menuItemRepository.GetMenuItemBy(id);
        }
    }
}
