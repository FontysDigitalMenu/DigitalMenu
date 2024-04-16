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

    public Order? GetByExternalPaymentId(string id)
    {
        return dbContext.Orders
            .Include(o => o.OrderMenuItems)
            .ThenInclude(omi => omi.MenuItem)
            .FirstOrDefault(o => o.ExternalPaymentId == id);
    }

    public Order? GetBy(string id, string deviceId, string tableId)
    {
        return dbContext.Orders
            .Include(o => o.OrderMenuItems)
            .ThenInclude(omi => omi.MenuItem)
            .FirstOrDefault(o => o.Id == id && o.DeviceId == deviceId && o.TableId == tableId);
    }

    public List<Order>? GetBy(string deviceId, string tableId)
    {
        return dbContext.Orders
            .Include(o => o.OrderMenuItems)
            .ThenInclude(omi => omi.MenuItem)
            .Where(o => o.DeviceId == deviceId && o.TableId == tableId)
            .ToList();
    }

    public IEnumerable<Order> GetPaidOrders()
    {
        return dbContext.Orders
            .Include(o => o.OrderMenuItems)
            .ThenInclude(omi => omi.MenuItem)
            .ThenInclude(mi => mi.Categories)
            .Where(o => o.PaymentStatus == PaymentStatus.Paid)
            .Where(o => o.Status == OrderStatus.Pending || o.Status == OrderStatus.Processing ||
                        o.Status == OrderStatus.Completed)
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
}