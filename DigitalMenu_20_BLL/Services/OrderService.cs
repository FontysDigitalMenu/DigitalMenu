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
    
    public bool Create(string deviceId, string tableId, string paymentId, int totalAmount)
    {
        if (!cartItemRepository.ExistsByDeviceId(deviceId))
        {
            return false;
        }

        if (tableRepository.GetById(tableId) == null)
        {
            return false;
        }
        
        
        List<CartItem> cartItems = cartItemRepository.GetByDeviceId(deviceId);

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
}