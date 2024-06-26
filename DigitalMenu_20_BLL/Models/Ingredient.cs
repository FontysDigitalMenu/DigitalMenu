﻿using System.ComponentModel.DataAnnotations.Schema;

namespace DigitalMenu_20_BLL.Models;

public class Ingredient
{
    public int Id { get; set; }

    public string Name { get; set; }

    public int Stock { get; set; }

    [NotMapped] public int Pieces { get; set; }

    public List<MenuItem> MenuItems { get; set; } = new();

    public List<IngredientTranslation> Translations { get; set; }

    public bool IsStockSufficient()
    {
        return Stock >= Pieces;
    }
}