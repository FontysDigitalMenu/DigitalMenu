using Microsoft.EntityFrameworkCore;

namespace DigitalMenu_20_BLL.Models;

[Keyless]
public class MenuItemIngredient
{
    public int MenuItemId { get; set; }

    public MenuItem MenuItem { get; set; }

    public int IngredientId { get; set; }

    public Ingredient Ingredient { get; set; }
}