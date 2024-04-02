using DigitalMenu_10_Api.ViewModels;

namespace DigitalMenu_10_Api.Hub;

public interface IOrderHubClient
{
    Task ReceiveOrder(OrderViewModel orderViewModel);
}