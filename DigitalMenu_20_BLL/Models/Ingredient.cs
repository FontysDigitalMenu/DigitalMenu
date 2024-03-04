using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalMenu_20_BLL.Models
{
    public class Ingredient
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int MenuItemId { get; set; }
        public MenuItem MenuItem { get; set; }
    }
}
