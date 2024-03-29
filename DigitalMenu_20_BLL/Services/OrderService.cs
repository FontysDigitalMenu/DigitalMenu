﻿using DigitalMenu_20_BLL.Exceptions;
using DigitalMenu_20_BLL.Interfaces.Repositories;
using DigitalMenu_20_BLL.Interfaces.Services;
using DigitalMenu_20_BLL.Models;
using Mollie.Api.Client;
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
        }).ToList();

        int totalAmount = GetTotalAmount(deviceId, tableId);

        Order order = new()
        {
            Id = orderId,
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

    public bool Update(Order order)
    {
        return orderRepository.Update(order);
    }

    public async Task<PaymentResponse> CreateMolliePayment(string apiKey, string redirectUrl, int totalAmount,
        string orderId)
    {
        using PaymentClient paymentClient = new($"{apiKey}", new HttpClient());
        PaymentRequest paymentRequest = new()
        {
            Amount = new Amount(Currency.EUR, (decimal)totalAmount / 100),
            Description = "Order payment",
            RedirectUrl = $"{redirectUrl}/{orderId}",
            Method = PaymentMethod.Ideal,
            // WebhookUrl = 
        };
        PaymentResponse paymentResponse = await paymentClient.CreatePaymentAsync(paymentRequest);

        return paymentResponse;
    }

    public async Task<PaymentResponse> GetPaymentFromMollie(string apiKey, string externalPaymentId)
    {
        using PaymentClient paymentClient = new(apiKey);
        try
        {
            return await paymentClient.GetPaymentAsync(externalPaymentId);
        }
        catch (MollieApiException e)
        {
            if (e.Details.Status == 404)
            {
                throw new NotFoundException("Payment not found");
            }

            throw new MollieApiException(e.Message);
        }
    }
}