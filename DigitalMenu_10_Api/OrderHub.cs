﻿using DigitalMenu_10_Api.ViewModels;
using DigitalMenu_20_BLL.Models;
using DigitalMenu_20_BLL.Services;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
using System.Text.RegularExpressions;

namespace DigitalMenu_10_Api
{
    public class OrderHub : Hub
    {
        //UserService _userService;
        //public OrderHub(UserService userService) { _userService = userService;}
        public async Task SendOrder(ReceiveOrderViewModel order)
        {
            //string email = Context?.User?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value!;

            //IdentityUser user = _userService.SearchByEmail(email).FirstOrDefault()!;

            await Clients.All.SendAsync("ReceiveOrder", order);
        }
    }
}


