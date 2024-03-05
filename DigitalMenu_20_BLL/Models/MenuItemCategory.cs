using Microsoft.EntityFrameworkCore;

namespace DigitalMenu_20_BLL.Models;

[Keyless]
public class MenuItemCategory
{
    public int MenuItemId { get; set; }

    public MenuItem MenuItem { get; set; }

    public int CategoryId { get; set; }

    public Category Category { get; set; }
}