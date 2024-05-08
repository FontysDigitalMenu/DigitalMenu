namespace DigitalMenu_10_Api.ViewModels;

public class CartItemViewModel
{
    public int Id { get; set; }

    public string? Note { get; set; }

    public int Quantity { get; set; }

    public string TableSessionId { get; set; }

    public int MenuItemId { get; set; }

    public MenuItemViewModel MenuItem { get; set; }

    public List<IngredientViewModel> ExcludedIngredients { get; set; }
}