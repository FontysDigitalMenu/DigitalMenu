namespace DigitalMenu_10_Api.ViewModels;

public class OrderViewModel
{
    public int Id { get; set; }

    public int TotalAmount { get; set; }

    public string Status { get; set; }

    public string PaymentStatus { get; set; }

    public DateTime OrderDate { get; set; } = DateTime.Now;

    public List<MenuItemViewModel> MenuItems { get; set; } = [];
}