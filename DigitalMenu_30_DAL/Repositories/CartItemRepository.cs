using DigitalMenu_20_BLL.Interfaces.Repositories;
using DigitalMenu_20_BLL.Models;
using DigitalMenu_30_DAL.Data;
using Microsoft.EntityFrameworkCore;

namespace DigitalMenu_30_DAL.Repositories;

public class CartItemRepository(ApplicationDbContext dbContext) : ICartItemRepository
{
    public bool Create(CartItem cartItem)
    {
        dbContext.CartItems.Add(cartItem);
        return dbContext.SaveChanges() > 0;
    }

    public CartItem? GetByMenuItemIdAndDeviceId(int menuItemId, string deviceId)
    {
        return dbContext.CartItems.FirstOrDefault(ci => ci.MenuItemId == menuItemId && ci.TableSessionId == deviceId);
    }

    public CartItem? GetByCartItemIdAndTableSessionId(int cartItemId, string tableSessionId)
    {
        return dbContext.CartItems
            .Include(ci => ci.MenuItem)
            .FirstOrDefault(ci => ci.Id == cartItemId && ci.TableSessionId == tableSessionId);
    }

    public List<CartItem> GetCartItemsByMenuItemIdAndTableSessionId(int menuItemId, string tableSessionId)
    {
        return dbContext.CartItems
            .Where(ci => ci.MenuItemId == menuItemId && ci.TableSessionId == tableSessionId)
            .ToList();
    }


    public List<CartItem> GetByTableSessionId(string tableSessionId)
    {
        return dbContext.CartItems
            .Include(ci => ci.MenuItem)
            .Where(ci => ci.TableSessionId == tableSessionId)
            .ToList();
    }

    public bool ExistsByTableSessionId(string tableSessionId)
    {
        return dbContext.CartItems.Any(ci => ci.TableSessionId == tableSessionId);
    }

    public bool Delete(CartItem cartItem)
    {
        dbContext.CartItems.Remove(cartItem);
        return dbContext.SaveChanges() > 0;
    }

    public bool Update(CartItem cartItem)
    {
        dbContext.CartItems.Update(cartItem);
        return dbContext.SaveChanges() > 0;
    }

    public bool AddExcludedIngredientToCartItem(ExcludedIngredientCartItem excludedIngredientCartItem)
    {
        try
        {
            dbContext.ExcludedIngredientCartItems.Add(excludedIngredientCartItem);
            dbContext.SaveChanges();

            return true;
        }
        catch
        {
            return false;
        }
    }

    public List<Ingredient> GetExcludedIngredientsByCartItemId(int cartItemId)
    {
        return dbContext.ExcludedIngredientCartItems
            .Where(e => e.CartItemId == cartItemId)
            .Select(e => e.Ingredient)
            .ToList();
    }

    public List<Ingredient> GetExcludedIngredientsByOrderMenuItemId(int orderMenuItemId)
    {
        return dbContext.ExcludedIngredientOrderMenuItems
            .Where(e => e.OrderMenuItemId == orderMenuItemId)
            .Select(e => e.Ingredient)
            .ToList();
    }

    public bool DeleteExcludedIngredientsFromCartItem(int cartItemId)
    {
        try
        {
            List<ExcludedIngredientCartItem> excludedIngredients = dbContext.ExcludedIngredientCartItems
                .Where(eic => eic.CartItemId == cartItemId)
                .ToList();

            dbContext.ExcludedIngredientCartItems.RemoveRange(excludedIngredients);
            dbContext.SaveChanges();

            return true;
        }
        catch
        {
            return false;
        }
    }

    public bool ClearByTableSessionId(string tableSessionId)
    {
        try
        {
            List<CartItem> cartItems = dbContext.CartItems.Where(ci => ci.TableSessionId == tableSessionId).ToList();
            dbContext.CartItems.RemoveRange(cartItems);
            dbContext.SaveChanges();

            return true;
        }
        catch
        {
            return false;
        }
    }
}