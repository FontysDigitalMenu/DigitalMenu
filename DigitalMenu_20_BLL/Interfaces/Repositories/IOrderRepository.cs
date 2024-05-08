using DigitalMenu_20_BLL.Models;

namespace DigitalMenu_20_BLL.Interfaces.Repositories;

public interface IOrderRepository
{
    public Order? Create(Order order);

    public Order? GetBy(string id, string tableSessionId);

    public Order? GetBy(string id);

    public List<Order>? GetByTableSessionId(string sessionId);

    public IEnumerable<Order> GetPaidOrders();

    public bool Update(Order order);

    public bool ExistsBySessionId(string sessionId);

    public List<Order> GetUnPaidOrders(string sessionId);
}