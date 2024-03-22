using DigitalMenu_20_BLL.Models;

namespace DigitalMenu_20_BLL.Interfaces.Services;

public interface ICartItemService
{
    public List<CartItem> GetAll();

    public CartItem GetById(int id);

    public void Create(CartItem cartItem);
}