namespace DigitalMenu_10_Api.ViewModels;

public class CartViewModel
{
    public List<CartItemViewModel> CartItems { get; set; }

    public int TotalAmount { get; set; }
}