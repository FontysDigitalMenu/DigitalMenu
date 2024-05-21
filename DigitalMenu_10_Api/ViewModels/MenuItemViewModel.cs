namespace DigitalMenu_10_Api.ViewModels;

public class MenuItemViewModel
{
    public int Id { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public int Price { get; set; }

    public string ImageUrl { get; set; }

    public int Quantity { get; set; }

    public string? Note { get; set; }

    public List<IngredientViewModel> Ingredients { get; set; } = [];

    public bool IsActive { get; set; } = true;
  
    public List<string>? Categories { get; set; }

    public List<IngredientViewModel> ExcludedIngredients { get; set; } = [];
}