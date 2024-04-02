using Mollie.Api.Models.Payment.Response;

namespace DigitalMenu_20_BLL.Helpers;

public interface IMollieHelper
{
    public Task<PaymentResponse> CreatePayment(int totalAmount, string orderId);

    public Task<PaymentResponse> GetPayment(string externalPaymentId);
}