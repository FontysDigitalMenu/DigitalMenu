using DigitalMenu_20_BLL.Interfaces.Repositories;
using DigitalMenu_20_BLL.Interfaces.Services;
using DigitalMenu_20_BLL.Models;

namespace DigitalMenu_20_BLL.Services;

public class CartItemService : ICartItemService
{
    private readonly ICartItemRepository _cartItemRepository;

    public CartItemService(ICartItemRepository cartItemRepository)
    {
        _cartItemRepository = cartItemRepository;
    }

    public List<CartItem> GetByDeviceId(string deviceId)
    {
        return _cartItemRepository.GetByDeviceId(deviceId);
    }

    public CartItem? GetByMenuItemIdAndDeviceId(int menuItemId, string deviceId)
    {
        return _cartItemRepository.GetByMenuItemIdAndDeviceId(menuItemId, deviceId);
    }

    public List<CartItem?> GetCartItemsByMenuItemIdAndDeviceId(int menuItemId, string deviceId)
    {
        return _cartItemRepository.GetCartItemsByMenuItemIdAndDeviceId(menuItemId, deviceId);
    }

    public bool Create(CartItem cartItem)
    {
        return _cartItemRepository.Create(cartItem);
    }

    public bool ExistsByDeviceId(string deviceId)
    {
        return _cartItemRepository.ExistsByDeviceId(deviceId) && !string.IsNullOrWhiteSpace(deviceId) && deviceId != "null";
    }

    public bool Delete(CartItem cartItem)
    {
        return _cartItemRepository.Delete(cartItem);
    }

    public bool Update(CartItem cartItem)
    {
        return _cartItemRepository.Update(cartItem);
    }

    public bool AddExcludedIngredientToCartItem(ExcludedIngredientCartItem excludedIngredientCartItem)
    {
        return _cartItemRepository.AddExcludedIngredientToCartItem(excludedIngredientCartItem);
    }

    public List<Ingredient> GetExcludedIngredientsByCartItemId(int cartItemId)
    {
        return _cartItemRepository.GetExcludedIngredientsByCartItemId(cartItemId);
    }

    public CartItem? GetByCartItemIdAndDeviceId(int cartItemId, string deviceId)
    {
        return _cartItemRepository.GetByCartItemIdAndDeviceId(cartItemId, deviceId);
    }

    public bool DeleteExcludedIngredientsFromCartItem(int cartItemId)
    {
        return _cartItemRepository.DeleteExcludedIngredientsFromCartItem(cartItemId);
    }
}