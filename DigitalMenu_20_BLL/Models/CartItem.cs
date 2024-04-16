namespace DigitalMenu_20_BLL.Models;

public class CartItem
{
    public int Id { get; set; }
    
    public string? Note { get; set; }
    
    public int Quantity { get; set; }
    
    public string DeviceId { get; set; }
    
    public int MenuItemId { get; set; }
    
    public MenuItem MenuItem { get; set; }
    
    public List<ExcludedIngredientCartItem> ExcludedIngredients { get; set; }
}