using Microsoft.AspNetCore.SignalR;

namespace DigitalMenu_10_Api.Hub;

public class OrderHub : Hub<IOrderHubClient>
{
    public async Task AddToGroup(GroupRequest groupRequest)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, groupRequest.GroupName);
    }
}