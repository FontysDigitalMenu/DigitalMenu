using DigitalMenu_20_BLL.Helpers;
using DigitalMenu_20_BLL.Interfaces.Repositories;
using DigitalMenu_20_BLL.Models;
using Mollie.Api.Models.Payment.Response;

namespace DigitalMenu_20_BLL.Services;

public class SplitService(ISplitRepository splitRepository, IMollieHelper mollieHelper) : ISplitService
{
    public Split? GetByExternalPaymentId(string id)
    {
        return splitRepository.GetByExternalPaymentId(id);
    }

    public async Task<PaymentResponse> GetPaymentFromMollie(string externalPaymentId)
    {
        return await mollieHelper.GetPayment(externalPaymentId);
    }

    public Split? Create(Split split)
    {
        return splitRepository.Create(split);
    }

    public bool Update(Split split)
    {
        return splitRepository.Update(split);
    }

    public Split? GetById(int id)
    {
        return splitRepository.GetById(id);
    }

    public async Task<PaymentResponse> CreateMolliePayment(Split split)
    {
        return await mollieHelper.CreatePayment(split);
    }
}