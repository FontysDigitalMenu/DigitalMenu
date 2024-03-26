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

    public bool ClearByDeviceId(string deviceId)
    {
        List<CartItem> cartItems = _dbContext.CartItems.Where(ci => ci.DeviceId == deviceId).ToList();
        _dbContext.CartItems.RemoveRange(cartItems);
        return _dbContext.SaveChanges() > 0;
    }
}