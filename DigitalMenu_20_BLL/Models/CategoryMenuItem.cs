namespace DigitalMenu_20_BLL.Models;

public class CategoryMenuItem
{
    public int Id { get; set; }

    public int MenuItemId { get; set; }

    public MenuItem MenuItem { get; set; }

    public int CategoryId { get; set; }

    public Category Category { get; set; }
}