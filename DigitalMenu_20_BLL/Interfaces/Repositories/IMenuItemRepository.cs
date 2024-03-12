using DigitalMenu_20_BLL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalMenu_20_BLL.Interfaces.Repositories
{
    public interface IMenuItemRepository
    {
        public IEnumerable<MenuItem> GetNextMenuItems(int lastId, int amount);
    }
}
