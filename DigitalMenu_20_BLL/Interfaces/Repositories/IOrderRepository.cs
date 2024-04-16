﻿using DigitalMenu_20_BLL.Models;

namespace DigitalMenu_20_BLL.Interfaces.Repositories;

public interface IOrderRepository
{
    public Order? Create(Order order);

    public Order? GetByExternalPaymentId(string id);

    public Order? GetBy(string id, string deviceId, string tableId);

    public Order? GetBy(string id);

    public List<Order>? GetBy(string deviceId, string tableId);

    public IEnumerable<Order> GetPaidOrders();

    public bool Update(Order order);

    public bool ExistsByDeviceId(string deviceId);
}