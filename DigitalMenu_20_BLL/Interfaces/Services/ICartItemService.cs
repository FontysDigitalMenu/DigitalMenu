using DigitalMenu_20_BLL.Models;

namespace DigitalMenu_20_BLL.Interfaces.Services;

public interface ICartItemService
{
    public List<CartItem> GetByTableSessionId(string tableSessionId);

    public CartItem? GetByMenuItemIdAndDeviceId(int menuItemId, string deviceId);

    public CartItem? GetByCartItemIdAndTableSessionId(int cartItemId, string tableSessionId);

    public List<CartItem> GetCartItemsByMenuItemIdAndTableSessionId(int menuItemId, string tableSessionId);

    public bool Create(CartItem cartItem);

    public bool ExistsByTableSessionId(string tableSessionId);

    public bool Delete(CartItem cartItem);

    public bool Update(CartItem cartItem);

    public bool AddExcludedIngredientToCartItem(ExcludedIngredientCartItem excludedIngredientCartItem);

    List<Ingredient> GetExcludedIngredientsByCartItemId(int cartItemId);

    List<Ingredient> GetExcludedIngredientsByOrderMenuItemId(int orderMenuItemId);

    public bool DeleteExcludedIngredientsFromCartItem(int cartItemId);

    public bool ClearByTableSessionId(string tableSessionId);
}