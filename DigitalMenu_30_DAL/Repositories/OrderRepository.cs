using DigitalMenu_20_BLL.Enums;
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
            .Include(o => o.Splits)
            .Include(o => o.OrderMenuItems)
            .ThenInclude(omi => omi.MenuItem)
            .FirstOrDefault(o => o.Id == id && o.DeviceId == deviceId && o.TableId == tableId);
    }

    public Order? GetBy(string id)
    {
        return dbContext.Orders
            .Include(o => o.Splits)
            .Include(o => o.OrderMenuItems)
            .ThenInclude(omi => omi.MenuItem)
            .FirstOrDefault(o => o.Id == id);
    }

    public List<Order>? GetByTableSessionId(string sessionId)
    {
        return dbContext.Orders
            .Include(o => o.Splits)
            .Include(o => o.OrderMenuItems)
            .ThenInclude(omi => omi.MenuItem)
            .Where(o => o.SessionId == sessionId)
            .OrderByDescending(o => o.OrderDate)
            .ToList();
    }

    public IEnumerable<Order> GetPaidOrders()
    {
        return dbContext.Orders
            .Include(o => o.Splits)
            .Include(o => o.OrderMenuItems)
            .ThenInclude(omi => omi.MenuItem)
            .ThenInclude(mi => mi.CategoryMenuItems)
            .ThenInclude(cm => cm.Category)
            .Where(o => o.Splits.All(s => s.PaymentStatus == PaymentStatus.Paid))
            .Where(o => o.FoodStatus == OrderStatus.Pending || o.FoodStatus == OrderStatus.Processing ||
                        o.FoodStatus == OrderStatus.Completed)
            .ToList();
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

    public bool ExistsBySessionId(string sessionId)
    {
        return dbContext.Orders.Any(o => o.SessionId == sessionId);
    }
}