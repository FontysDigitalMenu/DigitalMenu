using DigitalMenu_20_BLL.Enums;

namespace DigitalMenu_20_BLL.Models;

public class Split
{
    public int Id { get; set; }

    public string OrderId { get; set; }

    public Order Order { get; set; }

    public int Amount { get; set; }

    public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Pending;
}