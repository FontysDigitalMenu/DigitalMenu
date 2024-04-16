﻿using DigitalMenu_20_BLL.Interfaces.Services;
using DigitalMenu_20_BLL.Models;

namespace DigitalMenu_10_Api.ViewModels;

public class OrderViewModel
{
    public string Id { get; set; }

    public int TotalAmount { get; set; }

    public string Status { get; set; }

    public string PaymentStatus { get; set; }

    public DateTime OrderDate { get; set; } = DateTime.Now;

    public List<MenuItemViewModel> MenuItems { get; set; } = [];

    public string OrderNumber { get; set; }

    public static OrderViewModel FromOrder(Order order, ICartItemService cartItemService)
    {
        return new OrderViewModel
        {
            Id = order.Id,
            PaymentStatus = order.PaymentStatus.ToString(),
            Status = order.Status.ToString(),
            TotalAmount = order.TotalAmount,
            OrderDate = order.OrderDate,
            OrderNumber = order.OrderNumber,
            MenuItems = order.OrderMenuItems.Select(omi => new MenuItemViewModel
            {
                Id = omi.MenuItem.Id,
                Name = omi.MenuItem.Name,
                Price = omi.MenuItem.Price,
                ImageUrl = omi.MenuItem.ImageUrl,
                Quantity = omi.Quantity,
                Note = omi.Note,
                ExcludedIngredients = cartItemService.GetExcludedIngredientsByOrderMenuItemId(omi.Id).Select(i =>
                    new IngredientViewModel
                    {
                        Id = i.Id,
                        Name = i.Name,
                    }).ToList(),
            }).ToList(),
        };
    }
}