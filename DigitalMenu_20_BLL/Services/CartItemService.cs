using DigitalMenu_20_BLL.Interfaces.Repositories;
using DigitalMenu_20_BLL.Interfaces.Services;
using DigitalMenu_20_BLL.Models;

namespace DigitalMenu_20_BLL.Services;

public class CartItemService(ICartItemRepository cartItemRepository) : ICartItemService
{
    public List<CartItem> GetByTableSessionId(string tableSessionId)
    {
        return cartItemRepository.GetByTableSessionId(tableSessionId);
    }

    public CartItem? GetByMenuItemIdAndTableSessionId(int menuItemId, string tableSessionId)
    {
        return cartItemRepository.GetByMenuItemIdAndTableSessionId(menuItemId, tableSessionId);
    }

    public List<CartItem?> GetCartItemsByMenuItemIdAndTableSessionId(int menuItemId, string tableSessionId)
    {
        return cartItemRepository.GetCartItemsByMenuItemIdAndTableSessionId(menuItemId, tableSessionId);
    }

    public bool Create(CartItem cartItem)
    {
        return cartItemRepository.Create(cartItem);
    }

    public bool ExistsByTableSessionId(string tableSessionId)
    {
        return cartItemRepository.ExistsByTableSessionId(tableSessionId) && !string.IsNullOrWhiteSpace(tableSessionId) &&
               tableSessionId != "null";
    }

    public bool Delete(CartItem cartItem)
    {
        return cartItemRepository.Delete(cartItem);
    }

    public bool Update(CartItem cartItem)
    {
        return cartItemRepository.Update(cartItem);
    }

    public bool AddExcludedIngredientToCartItem(ExcludedIngredientCartItem excludedIngredientCartItem)
    {
        return cartItemRepository.AddExcludedIngredientToCartItem(excludedIngredientCartItem);
    }

    public List<Ingredient> GetExcludedIngredientsByCartItemId(int cartItemId)
    {
        return cartItemRepository.GetExcludedIngredientsByCartItemId(cartItemId);
    }

    public List<Ingredient> GetExcludedIngredientsByOrderMenuItemId(int orderMenuItemId)
    {
        return cartItemRepository.GetExcludedIngredientsByOrderMenuItemId(orderMenuItemId);
    }

    public CartItem? GetByCartItemIdAndTableSessionId(int cartItemId, string tableSessionId)
    {
        return cartItemRepository.GetByCartItemIdAndTableSessionId(cartItemId, tableSessionId);
    }

    public bool DeleteExcludedIngredientsFromCartItem(int cartItemId)
    {
        return cartItemRepository.DeleteExcludedIngredientsFromCartItem(cartItemId);
    }
}