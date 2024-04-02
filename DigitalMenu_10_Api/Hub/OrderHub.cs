using Microsoft.AspNetCore.SignalR;

namespace DigitalMenu_10_Api.Hub;

public class OrderHub : Hub<IOrderHubClient>;