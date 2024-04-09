namespace DigitalMenu_20_BLL.Models;

public class ExcludedIngredientOrderMenuItem
{
    public int Id { get; set; }

    public int IngredientId { get; set; }

    public Ingredient Ingredient { get; set; }

    public int OrderMenuItemId { get; set; }

    public OrderMenuItem OrderMenuItem { get; set; }
}