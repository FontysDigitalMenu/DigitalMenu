﻿using DigitalMenu_20_BLL.Exceptions;
using DigitalMenu_20_BLL.Models;
using Mollie.Api.Client;
using Mollie.Api.Models;
using Mollie.Api.Models.Payment;
using Mollie.Api.Models.Payment.Request;
using Mollie.Api.Models.Payment.Response;
using Serilog;

namespace DigitalMenu_20_BLL.Helpers;

public class MollieHelper(string apiKey, string redirectUrl, string backendUrl) : IMollieHelper
{
    public async Task<PaymentResponse> CreatePayment(Split split)
    {
        Log.Information("BackendUrl: {backendUrl}",
            backendUrl.Contains("localhost") ? null : $"{backendUrl}/api/v1/split/webhook");

        using PaymentClient paymentClient = new($"{apiKey}", new HttpClient());
        PaymentRequest paymentRequest = new()
        {
            Amount = new Amount(Currency.EUR, (decimal)split.Amount / 100),
            Description = "Order payment",
            RedirectUrl = $"{redirectUrl}/{split.OrderId}",
            Method = PaymentMethod.Ideal,
            WebhookUrl = backendUrl.Contains("localhost") ? null : $"{backendUrl}/api/v1/split/webhook",
        };
        Log.Information("PaymentRequest: {@paymentRequest}", paymentRequest);
        PaymentResponse paymentResponse = await paymentClient.CreatePaymentAsync(paymentRequest);

        return paymentResponse;
    }

    public async Task<PaymentResponse> GetPayment(string externalPaymentId)
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