using DigitalMenu_20_BLL.Interfaces.Repositories;
using DigitalMenu_20_BLL.Interfaces.Services;
using DigitalMenu_20_BLL.Models;

namespace DigitalMenu_20_BLL.Services;

public class CartItemService(ICartItemRepository cartItemRepository) : ICartItemService
{
    public List<CartItem> GetByDeviceId(string deviceId)
    {
        return cartItemRepository.GetByDeviceId(deviceId);
    }
    
    public CartItem? GetByMenuItemIdAndDeviceId(int menuItemId, string deviceId)
    {
        return cartItemRepository.GetByMenuItemIdAndDeviceId(menuItemId, deviceId);
    }
    
    public List<CartItem?> GetCartItemsByMenuItemIdAndDeviceId(int menuItemId, string deviceId)
    {
        return cartItemRepository.GetCartItemsByMenuItemIdAndDeviceId(menuItemId, deviceId);
    }
    
    public bool Create(CartItem cartItem)
    {
        return cartItemRepository.Create(cartItem);
    }
    
    public bool ExistsByDeviceId(string deviceId)
    {
        return cartItemRepository.ExistsByDeviceId(deviceId) && !string.IsNullOrWhiteSpace(deviceId) &&
               deviceId != "null";
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
    
    public CartItem? GetByCartItemIdAndDeviceId(int cartItemId, string deviceId)
    {
        return cartItemRepository.GetByCartItemIdAndDeviceId(cartItemId, deviceId);
    }
    
    public bool DeleteExcludedIngredientsFromCartItem(int cartItemId)
    {
        return cartItemRepository.DeleteExcludedIngredientsFromCartItem(cartItemId);
    }
}