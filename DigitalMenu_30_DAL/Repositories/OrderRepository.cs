using DigitalMenu_20_BLL.Interfaces.Repositories;
using DigitalMenu_20_BLL.Models;
using DigitalMenu_30_DAL.Data;

namespace DigitalMenu_30_DAL.Repositories;

public class OrderRepository(ApplicationDbContext dbContext) : IOrderRepository
{
    public bool Create(Order order)
    {
        dbContext.Orders.Add(order);
        return dbContext.SaveChanges() > 0;
    }
}