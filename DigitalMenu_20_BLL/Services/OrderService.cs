using DigitalMenu_20_BLL.Exceptions;
using DigitalMenu_20_BLL.Interfaces.Repositories;
using DigitalMenu_20_BLL.Interfaces.Services;
using DigitalMenu_20_BLL.Models;
using Mollie.Api.Client;
using Mollie.Api.Client.Abstract;
using Mollie.Api.Models;
using Mollie.Api.Models.Payment;
using Mollie.Api.Models.Payment.Request;
using Mollie.Api.Models.Payment.Response;

namespace DigitalMenu_20_BLL.Services;

public class OrderService(
    IOrderRepository orderRepository,
    ICartItemRepository cartItemRepository,
    ITableRepository tableRepository) : IOrderService
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

    public Order Create(string deviceId, string tableId, string paymentId)
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
        }).ToList();

        int totalAmount = GetTotalAmount(deviceId, tableId);

        Order order = new()
        {
            DeviceId = deviceId,
            TableId = tableId,
            ExternalPaymentId = paymentId,
            OrderMenuItems = orderMenuItems,
            TotalAmount = totalAmount,
        };

        Order? createdOrder = orderRepository.Create(order);
        if (createdOrder == null)
        {
            throw new DatabaseCreationException("Order could not be created");
        }

        if (!cartItemRepository.ClearByDeviceId(deviceId))
        {
            throw new DatabaseUpdateException("CartItems could not be cleared");
        }

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

    public async Task<PaymentResponse> CreateMolliePayment(string apiKey, string redirectUrl, int totalAmount)
    {
        using IPaymentClient paymentClient = new PaymentClient($"{apiKey}", new HttpClient());
        PaymentRequest paymentRequest = new()
        {
            Amount = new Amount(Currency.EUR, (decimal)totalAmount / 100),
            Description = "Order payment",
            RedirectUrl = redirectUrl,
            Method = PaymentMethod.Ideal,
        };
        PaymentResponse paymentResponse = await paymentClient.CreatePaymentAsync(paymentRequest);

        return paymentResponse;
    }
}