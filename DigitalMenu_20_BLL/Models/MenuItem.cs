namespace DigitalMenu_20_BLL.Models;

public class MenuItem
{
    public int Id { get; set; }

    public int OrderId { get; set; }

    public Order Order { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public int Price { get; set; }

    public string ImageUrl { get; set; }

    public List<Ingredient> Ingredients { get; set; }
}