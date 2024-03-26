using DigitalMenu_20_BLL.Models;

namespace DigitalMenu_20_BLL.Interfaces.Repositories;

public interface IOrderRepository
{
    public Order? Create(Order order);

    public Order? GetById(int id);

    public bool Update(Order order);
}