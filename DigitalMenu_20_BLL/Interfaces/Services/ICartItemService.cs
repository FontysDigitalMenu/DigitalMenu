using DigitalMenu_20_BLL.Models;

namespace DigitalMenu_20_BLL.Interfaces.Services;

public interface ICartItemService
{
    public List<CartItem> GetByDeviceId(string deviceId);

    public CartItem? GetByMenuItemIdAndDeviceId(int menuItemId, string deviceId);

    public bool Create(CartItem cartItem);

    public bool ExistsByDeviceId(string deviceId);

    public bool Delete(CartItem cartItem);

    public bool Update(CartItem cartItem);
}