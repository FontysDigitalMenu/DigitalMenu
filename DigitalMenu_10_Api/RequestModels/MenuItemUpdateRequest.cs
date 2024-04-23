namespace DigitalMenu_10_Api.RequestModels;

public class MenuItemUpdateRequest
{
    public int Id { get; set; }
    public string Name { get; set; }

    public decimal Price { get; set; }

    public string Description { get; set; }

    public List<string> Categories { get; set; }

    public List<string>? IngredientsName { get; set; }

    public List<int>? IngredientsAmount { get; set; }

    public IFormFile? Image { get; set; }
}