﻿namespace DigitalMenu_10_Api.RequestModels;

public class CartRequest
{
    public string TableSessionId { get; set; }

    public int MenuItemId { get; set; }

    public string? Note { get; set; }

    public List<string>? ExcludedIngredients { get; set; } = [];
}