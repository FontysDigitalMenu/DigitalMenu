using DigitalMenu_20_BLL.Models;

namespace DigitalMenu_20_BLL.Interfaces.Repositories;

public interface IOrderRepository
{
    public Order? Create(Order order);

    public Order? GetBy(string id, string deviceId, string tableId);

    public bool Update(Order order);

    public bool ExistsByDeviceId(string deviceId);
}