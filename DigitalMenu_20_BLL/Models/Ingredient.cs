using System.ComponentModel.DataAnnotations.Schema;

namespace DigitalMenu_20_BLL.Models;

public class Ingredient
{
    public int Id { get; set; }

    public string Name { get; set; }

    [NotMapped] public int Pieces { get; set; }

    public List<MenuItem> MenuItems { get; set; } = new();
}