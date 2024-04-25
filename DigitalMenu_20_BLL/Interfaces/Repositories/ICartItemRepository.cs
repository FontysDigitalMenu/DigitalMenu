﻿using DigitalMenu_20_BLL.Models;

namespace DigitalMenu_20_BLL.Interfaces.Repositories;

public interface ICartItemRepository
{
    public bool Create(CartItem cartItem);

    public CartItem? GetByMenuItemIdAndTableSessionId(int menuItemId, string tableSessionId);

    public CartItem? GetByCartItemIdAndTableSessionId(int cartItemId, string tableSessionId);

    public List<CartItem?> GetCartItemsByMenuItemIdAndTableSessionId(int menuItemId, string tableSessionId);

    public List<CartItem> GetByTableSessionId(string tableSessionId);

    public bool ExistsByTableSessionId(string tableSessionId);

    public bool Delete(CartItem cartItem);

    public bool Update(CartItem cartItem);

    public bool ClearByDeviceId(string deviceId);

    public bool AddExcludedIngredientToCartItem(ExcludedIngredientCartItem excludedIngredientCartItem);

    List<Ingredient> GetExcludedIngredientsByCartItemId(int cartItemId);

    List<Ingredient> GetExcludedIngredientsByOrderMenuItemId(int orderMenuItemId);

    public bool DeleteExcludedIngredientsFromCartItem(int cartItemId);
}