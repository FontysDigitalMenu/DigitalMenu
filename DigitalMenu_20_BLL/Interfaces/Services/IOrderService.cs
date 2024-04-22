using DigitalMenu_20_BLL.Models;

namespace DigitalMenu_20_BLL.Interfaces.Services;

public interface IOrderService
{
    public Order Create(string deviceId, string tableId);

    public List<Order>? GetBy(string deviceId, string tableId);

    public Order? GetBy(string id, string deviceId, string tableId);

    public Order? GetBy(string id);

    public IEnumerable<Order> GetPaidOrders();

    public IEnumerable<Order> GetPaidFoodOrders();

    public IEnumerable<Order> GetPaidDrinksOrders();

    public bool Update(Order order);

    public void ProcessPaidOrder(Order order);
}