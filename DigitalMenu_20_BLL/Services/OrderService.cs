using DigitalMenu_20_BLL.Interfaces;
using DigitalMenu_20_BLL.Interfaces.Repositories;
using DigitalMenu_20_BLL.Models;

namespace DigitalMenu_20_BLL.Services;

public class OrderService(IOrderRepository orderRepository, ICartItemRepository cartItemRepository, ITableRepository tableRepository) : IOrderService
{
    public int GetTotalAmount()
    {
        return 52377;
    }
    
    public Order? Create(string deviceId, string tableId, string paymentId, int totalAmount)
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
        
        Order order = new()
        {
            DeviceId = deviceId,
            TableId = tableId,
            ExternalPaymentId = paymentId,
            OrderMenuItems = orderMenuItems,
            TotalAmount = totalAmount,
        };
        
        return orderRepository.Create(order);
    }
    
    public Order? GetById (int id)
    {
        return orderRepository.GetById(id);
    }
}