﻿using Microsoft.EntityFrameworkCore;

namespace DigitalMenu_20_BLL.Models;

[Keyless]
public class OrderMenuItem
{
    public string OrderId { get; set; }

    public Order Order { get; set; }

    public int MenuItemId { get; set; }

    public MenuItem MenuItem { get; set; }

    public int Quantity { get; set; }

    public string? Note { get; set; }
}