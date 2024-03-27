using DigitalMenu_20_BLL.Exceptions;
using Mollie.Api.Client;
using Mollie.Api.Models;
using Mollie.Api.Models.Payment;
using Mollie.Api.Models.Payment.Request;
using Mollie.Api.Models.Payment.Response;

namespace DigitalMenu_20_BLL.Helpers;

public class MollieHelper(string apiKey, string redirectUrl) : IMollieHelper
{
    public async Task<PaymentResponse> CreatePayment(int totalAmount, string orderId)
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