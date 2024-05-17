using System.ComponentModel.DataAnnotations.Schema;

namespace DigitalMenu_20_BLL.Models;

public class MenuItem
{
    public int Id { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public int Price { get; set; }

    public string ImageUrl { get; set; }

    public List<CategoryMenuItem> CategoryMenuItems { get; set; }

    [NotMapped] public List<Category> Categories { get; set; }

    public List<Ingredient> Ingredients { get; set; } = [];

    public bool IsActive { get; set; } = true;

    public List<MenuItemTranslation>? Translations { get; set; }
}