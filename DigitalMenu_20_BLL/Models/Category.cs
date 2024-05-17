using System.ComponentModel.DataAnnotations.Schema;

namespace DigitalMenu_20_BLL.Models;

public class Category
{
    public int Id { get; set; }

    public string Name { get; set; }

    public List<CategoryMenuItem> CategoryMenuItems { get; set; }

    [NotMapped] public List<MenuItem> MenuItems { get; set; }

    public List<CategoryTranslation> Translations { get; set; }
}