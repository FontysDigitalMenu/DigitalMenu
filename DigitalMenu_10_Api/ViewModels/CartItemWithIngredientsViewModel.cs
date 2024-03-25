using DigitalMenu_20_BLL.Models;

namespace DigitalMenu_10_Api.ViewModels
{
    public class CartItemWithIngredientsViewModel
    {
        public MenuItem MenuItem { get; set; }

        public List<Ingredient> ExcludedIngredients { get; set; } = [];
    }
}
