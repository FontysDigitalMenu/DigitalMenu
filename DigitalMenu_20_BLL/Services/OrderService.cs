using DigitalMenu_20_BLL.Exceptions;
using DigitalMenu_20_BLL.Helpers;
using DigitalMenu_20_BLL.Interfaces.Repositories;
using DigitalMenu_20_BLL.Interfaces.Services;
using DigitalMenu_20_BLL.Models;
using Mollie.Api.Models.Payment.Response;
using shortid;
using shortid.Configuration;

namespace DigitalMenu_20_BLL.Services;

public class OrderService(
    IOrderRepository orderRepository,
    ICartItemRepository cartItemRepository,
    ITableRepository tableRepository,
    IMollieHelper mollieHelper) : IOrderService
{
    public int GetTotalAmount(string deviceId, string tableId)
    {
        if (!cartItemRepository.ExistsByDeviceId(deviceId))
        {
            throw new NotFoundException("DeviceId does not exist");
        }

        if (tableRepository.GetById(tableId) == null)
        {
            throw new NotFoundException("TableId does not exist");
        }

        List<CartItem> cartItems = cartItemRepository.GetByDeviceId(deviceId);
        if (cartItems.Count == 0)
        {
            throw new NotFoundException("CartItems do not exist");
        }

        return cartItems.Sum(item => item.MenuItem.Price * item.Quantity);
    }

    public Order Create(string deviceId, string tableId, string paymentId, string orderId)
    {
        if (!cartItemRepository.ExistsByDeviceId(deviceId))
        {
            throw new NotFoundException("DeviceId does not exist");
        }

        if (tableRepository.GetById(tableId) == null)
        {
            throw new NotFoundException("TableId does not exist");
        }

        List<CartItem> cartItems = cartItemRepository.GetByDeviceId(deviceId);
        if (cartItems.Count == 0)
        {
            throw new NotFoundException("CartItems do not exist");
        }

        List<OrderMenuItem> orderMenuItems = cartItems.Select(ci => new OrderMenuItem
        {
            MenuItemId = ci.MenuItemId,
            MenuItem = ci.MenuItem,
            Quantity = ci.Quantity,
            Note = ci.Note,
            ExcludedIngredientOrderMenuItems = cartItemRepository.GetExcludedIngredientsByCartItemId(ci.Id).Select(ei =>
                new ExcludedIngredientOrderMenuItem
                {
                    IngredientId = ei.Id,
                }).ToList(),
        }).ToList();

        int totalAmount = GetTotalAmount(deviceId, tableId);

        string orderNumber = DateTime.Now.ToString("ddyyMM") +
                             ShortId.Generate(new GenerationOptions(length: 8, useSpecialCharacters: false,
                                 useNumbers: false))[..4];
        Order order = new()
        {
            Id = orderId,
            DeviceId = deviceId,
            TableId = tableId,
            ExternalPaymentId = paymentId,
            OrderMenuItems = orderMenuItems,
            TotalAmount = totalAmount,
            OrderNumber = orderNumber,
        };

        Order? createdOrder = orderRepository.Create(order);
        if (createdOrder == null)
        {
            throw new DatabaseCreationException("Order could not be created");
        }

        return createdOrder;
    }

    public Order? GetByExternalPaymentId(string id)
    {
        return orderRepository.GetByExternalPaymentId(id);
    }

    public List<Order>? GetBy(string deviceId, string tableId)
    {
        if (!orderRepository.ExistsByDeviceId(deviceId))
        {
            throw new NotFoundException("DeviceId does not exist");
        }

        if (tableRepository.GetById(tableId) == null)
        {
            throw new NotFoundException("TableId does not exist");
        }

        return orderRepository.GetBy(deviceId, tableId);
    }

    public Order? GetBy(string id, string deviceId, string tableId)
    {
        if (!orderRepository.ExistsByDeviceId(deviceId))
        {
            throw new NotFoundException("DeviceId does not exist");
        }

        if (tableRepository.GetById(tableId) == null)
        {
            throw new NotFoundException("TableId does not exist");
        }

        return orderRepository.GetBy(id, deviceId, tableId);
    }

    public Order? GetBy(string id)
    {
        return orderRepository.GetBy(id);
    }

    public bool Update(Order order)
    {
        return orderRepository.Update(order);
    }

    public async Task<PaymentResponse> CreateMolliePayment(int totalAmount, string orderId)
    {
        return await mollieHelper.CreatePayment(totalAmount, orderId);
    }

    public async Task<PaymentResponse> GetPaymentFromMollie(string externalPaymentId)
    {
        return await mollieHelper.GetPayment(externalPaymentId);
    }

    public void ProcessPaidOrder(Order order)
    {
        if (!cartItemRepository.ClearByDeviceId(order.DeviceId))
        {
            throw new DatabaseUpdateException("CartItems could not be cleared");
        }
    }

    public IEnumerable<Order> GetPaidOrders()
    {
        return orderRepository.GetPaidOrders();
    }
}