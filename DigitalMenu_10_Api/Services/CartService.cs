﻿using DigitalMenu_10_Api.ViewModels;
using DigitalMenu_20_BLL.Interfaces.Services;
using DigitalMenu_20_BLL.Models;

namespace DigitalMenu_10_Api.Services;

public static class CartService
{
    public static CartViewModel GetCartViewModel(IOrderService orderService, ICartItemService cartItemService,
        string tableSessionId)
    {
        List<Order> unpaidOrders = orderService.GetUnpaidOrdersByTableSessionId(tableSessionId);

        List<CartItem> cartItems = cartItemService.GetByTableSessionId(tableSessionId);

        return new CartViewModel
        {
            AnyUnpaidOrders = unpaidOrders.Count > 0,
            CartItems = cartItems.Select(cartItem => new CartItemViewModel
            {
                Id = cartItem.Id,
                Note = cartItem.Note,
                Quantity = cartItem.Quantity,
                TableSessionId = cartItem.TableSessionId,
                MenuItemId = cartItem.MenuItemId,
                MenuItem = new MenuItemViewModel
                {
                    Id = cartItem.MenuItem.Id,
                    Name = cartItem.MenuItem.Name,
                    Price = cartItem.MenuItem.Price,
                    ImageUrl = cartItem.MenuItem.ImageUrl,
                },
            }).ToList(),
            TotalAmount = cartItems.Sum(item => item.MenuItem.Price * item.Quantity),
        };
    }
}