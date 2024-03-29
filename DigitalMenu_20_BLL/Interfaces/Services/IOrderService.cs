﻿using DigitalMenu_20_BLL.Models;
using Mollie.Api.Models.Payment.Response;

namespace DigitalMenu_20_BLL.Interfaces.Services;

public interface IOrderService
{
    public int GetTotalAmount(string deviceId, string tableId);

    public Order Create(string deviceId, string tableId, string paymentId, string orderId);

    public Order? GetBy(string id, string deviceId, string tableId);

    public bool Update(Order order);

    public Task<PaymentResponse>
        CreateMolliePayment(string apiKey, string redirectUrl, int totalAmount, string orderId);

    public Task<PaymentResponse> GetPaymentFromMollie(string apiKey, string externalPaymentId);
}