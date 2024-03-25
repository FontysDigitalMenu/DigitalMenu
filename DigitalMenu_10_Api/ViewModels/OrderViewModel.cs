using DigitalMenu_20_BLL.Models;

namespace DigitalMenu_10_Api.ViewModels;

public class OrderViewModel
{
    public string PaymentStatus { get; set; }

    public Order Order { get; set; }
}