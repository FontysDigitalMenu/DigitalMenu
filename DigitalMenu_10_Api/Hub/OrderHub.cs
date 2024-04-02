using DigitalMenu_10_Api.ViewModels;
using DigitalMenu_20_BLL.Models;
using Microsoft.AspNetCore.SignalR;

namespace DigitalMenu_10_Api.Hub;

public class OrderHub : Hub<IMainHubClient>
{
    public async Task SendOrder(Order order)
    {
        OrderViewModel orderViewModel = new()
        {
            Id = order.Id,
            PaymentStatus = order.PaymentStatus.ToString(),
            Status = order.Status.ToString(),
            TotalAmount = order.TotalAmount,
            OrderDate = order.OrderDate,
            MenuItems = order.OrderMenuItems.Select(omi => new MenuItemViewModel
            {
                Id = omi.MenuItem.Id,
                Name = omi.MenuItem.Name,
                Price = omi.MenuItem.Price,
                ImageUrl = omi.MenuItem.ImageUrl,
            }).ToList(),
        };

        await Clients.All.ReceiveOrder(orderViewModel);
    }
}