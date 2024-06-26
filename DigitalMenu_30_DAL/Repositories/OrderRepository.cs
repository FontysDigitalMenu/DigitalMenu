﻿using DigitalMenu_20_BLL.Enums;
using DigitalMenu_20_BLL.Interfaces.Repositories;
using DigitalMenu_20_BLL.Interfaces.Services;
using DigitalMenu_20_BLL.Models;
using DigitalMenu_30_DAL.Data;
using Microsoft.EntityFrameworkCore;

namespace DigitalMenu_30_DAL.Repositories;

public class OrderRepository(ApplicationDbContext dbContext, ITimeService timeService) : IOrderRepository
{
    public Order? Create(Order order)
    {
        order.OrderDate = timeService.GetNow();
        dbContext.Orders.Add(order);
        return dbContext.SaveChanges() > 0 ? order : null;
    }

    public Order? GetBy(string id, string tableSessionId)
    {
        return dbContext.Orders
            .Include(o => o.Splits)
            .Include(o => o.OrderMenuItems)
            .ThenInclude(omi => omi.MenuItem)
            .ThenInclude(mi => mi.CategoryMenuItems)
            .ThenInclude(cmi => cmi.Category)
            .FirstOrDefault(o => o.Id == id && o.SessionId == tableSessionId);
    }

    public Order? GetBy(string id)
    {
        return dbContext.Orders
            .Include(o => o.Table)
            .Include(o => o.Splits)
            .Include(o => o.OrderMenuItems)
            .ThenInclude(omi => omi.MenuItem)
            .ThenInclude(mi => mi.CategoryMenuItems)
            .ThenInclude(cmi => cmi.Category)
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
            .Include(o => o.Table)
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
        order.OrderDate = timeService.GetNow();
        dbContext.Orders.Update(order);
        return dbContext.SaveChanges() > 0;
    }

    public bool ExistsBySessionId(string sessionId)
    {
        return dbContext.Orders.Any(o => o.SessionId == sessionId);
    }

    public List<Order> GetUnPaidOrders(string sessionId)
    {
        return dbContext.Orders
            .Where(o => o.SessionId == sessionId)
            .Where(o => o.Splits.Any(s => s.PaymentStatus != PaymentStatus.Paid))
            .ToList();
    }
}