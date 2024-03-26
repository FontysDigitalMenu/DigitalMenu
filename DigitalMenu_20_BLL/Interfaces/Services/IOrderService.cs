using DigitalMenu_20_BLL.Models;

namespace DigitalMenu_20_BLL.Interfaces.Services;

public interface IOrderService
{
    public int? GetTotalAmount(string deviceId, string tableId);

    public Order? Create(string deviceId, string tableId, string paymentId);

    public Order? GetById(int id);

    public bool Update(Order order);
}