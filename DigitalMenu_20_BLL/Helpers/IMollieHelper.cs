using DigitalMenu_20_BLL.Models;
using Mollie.Api.Models.Payment.Response;

namespace DigitalMenu_20_BLL.Helpers;

public interface IMollieHelper
{
    public Task<PaymentResponse> CreatePayment(Split split);

    public Task<PaymentResponse> GetPayment(string externalPaymentId);
}