using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DigitalMenu_20_BLL.Interfaces.Repositories;
using DigitalMenu_20_BLL.Models;
using DigitalMenu_30_DAL.Data;


namespace DigitalMenu_30_DAL.Repositories
{
    public class MenuItemRepository: IMenuItemRepository
    {
        private readonly ApplicationDbContext _dbContext;
        public MenuItemRepository(ApplicationDbContext dbContext) 
        {
            _dbContext = dbContext;
        }

        public IEnumerable<MenuItem> GetNextMenuItems(int lastId, int amount)
        {
            return _dbContext.MenuItems
                .OrderBy(m => m.Id)
                .Where(m => m.Id > lastId)
                .Take(amount)
                .ToList();
        }
    }
}
