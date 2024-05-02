using Microsoft.AspNetCore.SignalR;

namespace DigitalMenu_10_Api.Hub;

public class OrderHub : Hub<IOrderHubClient>
{
    public async Task AddToOrderGroup(OrderGroupRequest orderGroupRequest)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, orderGroupRequest.GroupName);
    }
}