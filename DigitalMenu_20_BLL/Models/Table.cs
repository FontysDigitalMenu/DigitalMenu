﻿namespace DigitalMenu_20_BLL.Models;

public class Table
{
    public string Id { get; set; }

    public string Name { get; set; }

    public List<Order> Orders { get; set; }

    public string QrCode { get; set; }

    public DateTime CreatedAt { get; set; }
}