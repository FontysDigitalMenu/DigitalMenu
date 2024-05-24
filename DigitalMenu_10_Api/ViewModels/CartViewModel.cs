namespace DigitalMenu_10_Api.ViewModels;

public class CartViewModel
{
    public bool AnyUnpaidOrders { get; set; }

    public List<CartItemViewModel> CartItems { get; set; }

    public int TotalAmount { get; set; }
    
    public int ReservationFee { get; set; }
}