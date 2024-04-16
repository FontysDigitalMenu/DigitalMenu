namespace DigitalMenu_20_BLL.Models;

public class Ingredient
{
    public int Id { get; set; }
    
    public string Name { get; set; }
    
    public List<MenuItem> MenuItems { get; set; } = new();
}