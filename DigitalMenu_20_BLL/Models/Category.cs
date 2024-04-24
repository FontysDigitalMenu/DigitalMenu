namespace DigitalMenu_20_BLL.Models;

public class Category
{
    public int Id { get; set; }

    public string Name { get; set; }

    public List<CategoryMenuItem> CategoryMenuItems { get; set; }

    public List<MenuItem> MenuItems { get; set; }
}