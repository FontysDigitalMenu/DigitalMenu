namespace DigitalMenu_20_BLL.Models;

public class ExcludedIngredientCartItem
{
    public int Id { get; set; }
    
    public int IngredientId { get; set; }
    
    public Ingredient Ingredient { get; set; }
    
    public int CartItemId { get; set; }
    
    public CartItem CartItem { get; set; }
}