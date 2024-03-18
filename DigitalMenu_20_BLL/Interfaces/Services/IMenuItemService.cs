using DigitalMenu_20_BLL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalMenu_20_BLL.Interfaces.Services
{
    public interface IMenuItemService
    {
        public IEnumerable<MenuItem> GetNextMenuItems(int lastId, int amount);
        public IEnumerable<Category> GetCategoriesWithNextMenuItems(int lastId, int amount);

    }
}
