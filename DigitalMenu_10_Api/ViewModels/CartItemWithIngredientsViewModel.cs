using DigitalMenu_20_BLL.Models;

namespace DigitalMenu_10_Api.ViewModels;

public class CartItemWithIngredientsViewModel
{
    public CartItemViewModel CartItem { get; set; }

    public List<IngredientViewModel> ExcludedIngredients { get; set; } = [];
}