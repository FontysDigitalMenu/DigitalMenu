namespace DigitalMenu_10_Api.RequestModels;

public class CartItemRequest
{
    public string DeviceId { get; set; }
    
    public int CartItemId { get; set; }
    
    public string? Note { get; set; }
    
    public List<string>? ExcludedIngredients { get; set; } = [];
}