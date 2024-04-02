using DigitalMenu_10_Api.ViewModels;
using DigitalMenu_20_BLL.Interfaces.Services;
using DigitalMenu_20_BLL.Models;
using Microsoft.AspNetCore.SignalR;

namespace DigitalMenu_10_Api;

public class OrderHub : Hub
{
    private readonly IOrderService _orderService;

    private IUserService _userService;

    public OrderHub(IUserService userService, IOrderService orderService)
    {
        _userService = userService;
        _orderService = orderService;
    }

    public async Task SendOrder(int orderId)
    {
        //string email = Context?.User?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value!;

        //IdentityUser user = _userService.SearchByEmail(email).FirstOrDefault()!;

        Order? order = _orderService.GetById(orderId);

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
        await Clients.All.SendAsync("ReceiveOrder", orderViewModel);
    }
}