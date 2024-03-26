using DigitalMenu_20_BLL.Interfaces.Repositories;
using DigitalMenu_20_BLL.Models;
using DigitalMenu_30_DAL.Data;
using Microsoft.EntityFrameworkCore;

namespace DigitalMenu_30_DAL.Repositories;

public class OrderRepository(ApplicationDbContext dbContext) : IOrderRepository
{
    public Order? Create(Order order)
    {
        dbContext.Orders.Add(order);
        return dbContext.SaveChanges() > 0 ? order : null;
    }

    public Order? GetBy(string id, string deviceId, string tableId)
    {
        return dbContext.Orders
            .Include(o => o.OrderMenuItems)
            .ThenInclude(omi => omi.MenuItem)
            .FirstOrDefault(o => o.Id == id && o.DeviceId == deviceId && o.TableId == tableId);
    }

    public bool Update(Order order)
    {
        dbContext.Orders.Update(order);
        return dbContext.SaveChanges() > 0;
    }

    public bool ExistsByDeviceId(string deviceId)
    {
        return dbContext.Orders.Any(o => o.DeviceId == deviceId);
    }
}