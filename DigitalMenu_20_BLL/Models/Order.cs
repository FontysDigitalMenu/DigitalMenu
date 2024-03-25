using DigitalMenu_20_BLL.Enums;

namespace DigitalMenu_20_BLL.Models;

public class Order
{
    public int Id { get; set; }

    public string DeviceId { get; set; }

    public string TableId { get; set; }

    public Table Table { get; set; }

    public string Note { get; set; }

    public int TotalAmount { get; set; }

    public int Quantity { get; set; }

    public OrderStatus Status { get; set; }

    public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Pending;

    public DateTime OrderDate { get; set; } = DateTime.Now;

    public List<OrderMenuItem> OrderMenuItems { get; set; } = [];
}