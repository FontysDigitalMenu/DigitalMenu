using DigitalMenu_20_BLL.Interfaces.Repositories;
using DigitalMenu_20_BLL.Interfaces.Services;
using DigitalMenu_20_BLL.Models;

namespace DigitalMenu_20_BLL.Services;

public class OrderService(
    IOrderRepository orderRepository,
    ICartItemRepository cartItemRepository,
    ITableRepository tableRepository) : IOrderService
{
    public int? GetTotalAmount(string deviceId, string tableId)
    {
        if (!cartItemRepository.ExistsByDeviceId(deviceId))
        {
            return null;
        }

        if (tableRepository.GetById(tableId) == null)
        {
            return null;
        }

        List<CartItem> cartItems = cartItemRepository.GetByDeviceId(deviceId);
        if (cartItems.Count == 0)
        {
            return null;
        }

        return cartItems.Sum(item => item.MenuItem.Price * item.Quantity);
    }

    public Order? Create(string deviceId, string tableId, string paymentId)
    {
        if (!cartItemRepository.ExistsByDeviceId(deviceId))
        {
            return null;
        }

        if (tableRepository.GetById(tableId) == null)
        {
            return null;
        }

        List<CartItem> cartItems = cartItemRepository.GetByDeviceId(deviceId);
        if (cartItems.Count == 0)
        {
            return null;
        }

        List<OrderMenuItem> orderMenuItems = cartItems.Select(ci => new OrderMenuItem
        {
            MenuItemId = ci.MenuItemId,
            MenuItem = ci.MenuItem,
        }).ToList();

        int? totalAmount = GetTotalAmount(deviceId, tableId);
        if (totalAmount == null)
        {
            return null;
        }

        Order order = new()
        {
            DeviceId = deviceId,
            TableId = tableId,
            ExternalPaymentId = paymentId,
            OrderMenuItems = orderMenuItems,
            TotalAmount = totalAmount.Value,
        };

        Order? createdOrder = orderRepository.Create(order);

        cartItemRepository.ClearByDeviceId(deviceId);

        return createdOrder;
    }

    public Order? GetById(int id)
    {
        return orderRepository.GetById(id);
    }

    public bool Update(Order order)
    {
        return orderRepository.Update(order);
    }
}