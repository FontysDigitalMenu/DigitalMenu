using DigitalMenu_20_BLL.Models;

namespace DigitalMenu_20_BLL.Interfaces.Services;

public interface IOrderService
{
    public Order Create(string tableSessionId, List<Split> splits);

    public List<Order>? GetByTableSessionId(string tableSessionId);

    public Order? GetBy(string id, string tableSessionId);

    public Order? GetBy(string id);

    public IEnumerable<Order> GetPaidOrders();

    public IEnumerable<Order> GetPaidFoodOrders();

    public IEnumerable<Order> GetPaidDrinksOrders();

    public bool Update(Order order);

    public List<Order> GetUnpaidOrdersByTableSessionId(string tableSessionId);
}