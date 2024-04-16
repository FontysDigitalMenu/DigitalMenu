namespace DigitalMenu_20_BLL.Models;

public class OrderMenuItem
{
    public int Id { get; set; }
    
    public string OrderId { get; set; }
    
    public Order Order { get; set; }
    
    public int MenuItemId { get; set; }
    
    public MenuItem MenuItem { get; set; }
    
    public int Quantity { get; set; }
    
    public string? Note { get; set; }
    
    public List<ExcludedIngredientOrderMenuItem> ExcludedIngredientOrderMenuItems { get; set; }
}