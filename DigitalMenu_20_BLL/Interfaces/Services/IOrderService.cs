using DigitalMenu_20_BLL.Models;

namespace DigitalMenu_20_BLL.Interfaces;

public interface IOrderService
{
    public int GetTotalAmount();

    public Order? Create(string deviceId, string tableId, string paymentId, int totalAmount);

    public Order? GetById(int id);
}