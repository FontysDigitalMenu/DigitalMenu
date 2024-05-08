using DigitalMenu_20_BLL.Enums;

namespace DigitalMenu_20_BLL.Models;

public class Order
{
    public string Id { get; set; }

    public string TableId { get; set; }

    public Table Table { get; set; }

    public string SessionId { get; set; }

    public int TotalAmount { get; set; }

    public OrderStatus FoodStatus { get; set; } = OrderStatus.Pending;

    public OrderStatus DrinkStatus { get; set; } = OrderStatus.Pending;

    public DateTime OrderDate { get; set; } = DateTime.Now;

    public List<OrderMenuItem> OrderMenuItems { get; set; } = [];

    public string OrderNumber { get; set; }

    public List<Split> Splits { get; set; }
}