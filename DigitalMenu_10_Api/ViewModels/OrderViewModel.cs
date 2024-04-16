namespace DigitalMenu_10_Api.ViewModels;

public class OrderViewModel
{
    public string Id { get; set; }
    
    public int TotalAmount { get; set; }
    
    public string Status { get; set; }
    
    public string PaymentStatus { get; set; }
    
    public DateTime OrderDate { get; set; } = DateTime.Now;
    
    public List<MenuItemViewModel> MenuItems { get; set; } = [];
    
    public string OrderNumber { get; set; }
}