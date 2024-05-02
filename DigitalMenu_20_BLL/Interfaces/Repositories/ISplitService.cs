using DigitalMenu_20_BLL.Models;
using Mollie.Api.Models.Payment.Response;

namespace DigitalMenu_20_BLL.Interfaces.Repositories;

public interface ISplitService
{
    public Split? GetByExternalPaymentId(string id);

    public Task<PaymentResponse> GetPaymentFromMollie(string externalPaymentId);

    public Task<PaymentResponse> CreateMolliePayment(Split split);

    public Split? Create(Split split);

    public bool Update(Split split);

    public Split? GetById(int id);
}