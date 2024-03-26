using DigitalMenu_20_BLL.Interfaces.Repositories;
using DigitalMenu_20_BLL.Models;
using DigitalMenu_30_DAL.Data;
using Microsoft.EntityFrameworkCore;

namespace DigitalMenu_30_DAL.Repositories;

public class CartItemRepository : ICartItemRepository
{
    private readonly ApplicationDbContext _dbContext;

    public CartItemRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public bool Create(CartItem cartItem)
    {
        _dbContext.CartItems.Add(cartItem);
        return _dbContext.SaveChanges() > 0;
    }

    public CartItem? GetByMenuItemIdAndDeviceId(int menuItemId, string deviceId)
    {
        return _dbContext.CartItems.FirstOrDefault(ci => ci.MenuItemId == menuItemId && ci.DeviceId == deviceId);
    }

    public CartItem? GetByCartItemIdAndDeviceId(int cartItemId, string deviceId)
    {
        return _dbContext.CartItems
            .Include(ci => ci.MenuItem)
            .FirstOrDefault(ci => ci.Id == cartItemId && ci.DeviceId == deviceId);
    }

    public List<CartItem?> GetCartItemsByMenuItemIdAndDeviceId(int menuItemId, string deviceId)
    {
        return _dbContext.CartItems
                      .Where(ci => ci.MenuItemId == menuItemId && ci.DeviceId == deviceId)
                      .ToList();
    }


    public List<CartItem> GetByDeviceId(string deviceId)
    {
        return _dbContext.CartItems
            .Include(ci => ci.MenuItem)
            .Where(ci => ci.DeviceId == deviceId)
            .ToList();
    }

    public bool ExistsByDeviceId(string deviceId)
    {
        return _dbContext.CartItems.Any(ci => ci.DeviceId == deviceId);
    }

    public bool Delete(CartItem cartItem)
    {
        _dbContext.CartItems.Remove(cartItem);
        return _dbContext.SaveChanges() > 0;
    }

    public bool Update(CartItem cartItem)
    {
        _dbContext.CartItems.Update(cartItem);
        return _dbContext.SaveChanges() > 0;
    }

    public bool AddExcludedIngredientToCartItem(ExcludedIngredientCartItem excludedIngredientCartItem)
    {
        try
        {
            _dbContext.ExcludedIngredientCartItems.Add(excludedIngredientCartItem);
            _dbContext.SaveChanges();

            return true;
        }
        catch
        {
            return false;
        }
    }

    public List<Ingredient> GetExcludedIngredientsByCartItemId(int cartItemId)
    {
        return _dbContext.ExcludedIngredientCartItems
            .Where(e => e.CartItemId == cartItemId)
            .Select(e => e.Ingredient)
            .ToList();
    }

    public bool DeleteExcludedIngredientsFromCartItem(int cartItemId)
    {
        try
        {
            List<ExcludedIngredientCartItem> excludedIngredients = _dbContext.ExcludedIngredientCartItems
                .Where(eic => eic.CartItemId == cartItemId)
                .ToList();

            _dbContext.ExcludedIngredientCartItems.RemoveRange(excludedIngredients);
            _dbContext.SaveChanges();

            return true;
        }
        catch
        {
            return false;
        }
    }

    public bool ClearByDeviceId(string deviceId)
    {
        List<CartItem> cartItems = _dbContext.CartItems.Where(ci => ci.DeviceId == deviceId).ToList();
        _dbContext.CartItems.RemoveRange(cartItems);
        return _dbContext.SaveChanges() > 0;
    }
}