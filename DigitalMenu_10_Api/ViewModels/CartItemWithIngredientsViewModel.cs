namespace DigitalMenu_10_Api.ViewModels;

public class CartItemWithIngredientsViewModel
{
    public CartItemViewModel CartItem { get; set; }

    public List<IngredientViewModel> ExcludedIngredients { get; set; } = [];
}